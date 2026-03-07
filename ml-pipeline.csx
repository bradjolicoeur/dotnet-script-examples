#r "nuget: Microsoft.ML, 3.0.1"
#r "nuget: Microsoft.ML.AutoML, 0.21.1"

using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;

var dataset = "data/home-sale-prices-10000.csv";
var testDataset = "data/home-sale-prices-1000.csv";

// Initialize MLContext
MLContext ctx = new MLContext();

// Infer column information
ColumnInferenceResults columnInference =
    ctx.Auto().InferColumns(testDataset, labelColumnName: "CurrentPrice", groupColumns: false);

// Create text loader
TextLoader loader = ctx.Data.CreateTextLoader(columnInference.TextLoaderOptions);

// Load data into IDataView
IDataView data = loader.Load(testDataset);

TrainTestData trainValidationData = ctx.Data.TrainTestSplit(data, testFraction: 0.2);

SweepablePipeline pipeline =
    ctx.Auto().Featurizer(data, columnInformation: columnInference.ColumnInformation)
        .Append(ctx.Auto().Regression(labelColumnName: columnInference.ColumnInformation.LabelColumnName));

AutoMLExperiment experiment = ctx.Auto().CreateExperiment();

experiment
    .SetPipeline(pipeline)
    .SetRegressionMetric(RegressionMetric.RSquared, labelColumn: columnInference.ColumnInformation.LabelColumnName)
    .SetTrainingTimeInSeconds(60)
    .SetDataset(trainValidationData);

// Log experiment trials
ctx.Log += (_, e) => {
    if (e.Source.Equals("AutoMLExperiment"))
    {
        Console.WriteLine(e.RawMessage);
    }
};

TrialResult experimentResults = await experiment.RunAsync();

// Get best model
var model = experimentResults.Model;

// Save Trained Model
ctx.Model.Save(model, data.Schema, "data/model.zip");


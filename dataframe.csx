#r "nuget: Microsoft.Data.Analysis, 0.21.1"

using System.IO;
using System.Linq;
using Microsoft.Data.Analysis;

// Define data path
var dataPath = Path.GetFullPath(@"data/home-sale-prices.csv");

// Load the data into the data frame
var dataFrame = DataFrame.LoadCsv(dataPath);

// output a description of the data loaded
Console.WriteLine(dataFrame.Description());

// Filter for prices over 200,000
PrimitiveDataFrameColumn<bool> boolFilter = dataFrame["CurrentPrice"].ElementwiseLessThan(250000);
DataFrame filteredDataFrame = dataFrame.Filter(boolFilter);

Console.WriteLine(filteredDataFrame.Description());

// Save the filtered output to a csv file
DataFrame.SaveCsv(filteredDataFrame, "data/result.csv", ',');
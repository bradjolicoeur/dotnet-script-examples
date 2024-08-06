#r "nuget: Newtonsoft.Json, 12.0.1"

using System.Net.Http;
using Newtonsoft.Json;

var client = new HttpClient();

string apiURL = "https://v2.jokeapi.dev/joke/Programming?blacklistFlags=nsfw,religious,political,racist,sexist,explicit";
var response = (await client.GetAsync(apiURL));
var json = (await response.Content.ReadAsStringAsync());

dynamic content = JsonConvert.DeserializeObject(json);

if(content.type == "twopart")
{
    Console.WriteLine($"{content.setup}");
    await Task.Delay(2500);
    Console.WriteLine($"-> {content.delivery}");
}
else
{
    Console.WriteLine($"{content.joke}");
}
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var client = new HttpClient { BaseAddress = new Uri("http://172.10.38.27:5050") };

var testJson = @"
{
    ""aboneTipi"": ""BIREYSEL"",
    ""ad"": ""API TEST"",
    ""soyad"": ""USER"",
    ""telefon"": ""05550001122"",
    ""ePosta"": ""test1@mail.com"",
    ""email"": ""test2@mail.com"",
    ""mail"": ""test3@mail.com"",
    ""e_posta"": ""test4@mail.com"",
    ""eposta"": ""test5@mail.com"",
    ""Email"": ""test6@mail.com"",
    ""EPosta"": ""test7@mail.com""
}";

var content = new StringContent(testJson, Encoding.UTF8, "application/json");
var response = await client.PostAsync("/api/Aboneler", content);
Console.WriteLine($"POST Response: {response.StatusCode}");
if (!response.IsSuccessStatusCode) {
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

var getResponse = await client.GetStringAsync("/api/Aboneler/all");
var root = JsonDocument.Parse(getResponse);
foreach (var el in root.RootElement.EnumerateArray()) {
    if (el.TryGetProperty("ad", out var ad) && ad.GetString() == "API TEST") {
        Console.WriteLine("Found Test User!");
        Console.WriteLine(el.ToString());
    }
}

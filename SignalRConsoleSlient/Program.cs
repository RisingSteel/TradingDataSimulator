using Core.Models;
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5224/hubs/prices")
    .WithAutomaticReconnect()
    .Build();

connection.On<PriceTick>("PriceUpdated", priceTick =>
{
    Console.WriteLine($"SignalR console - {priceTick.Timestamp} : Price for {priceTick.Symbol} is {priceTick.Price}");
});

await connection.StartAsync();

Console.WriteLine("SignalR client started. Listening for messages...");
Console.WriteLine("");

await Task.Delay(-1);
using Core.Models;
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5224/hubs/prices")
    .WithAutomaticReconnect()
    .Build();

connection.On<PriceTick>("PriceUpdated", priceTick =>
{
    Console.WriteLine($"{priceTick.Timestamp} : Price for {priceTick.Symbol} is {priceTick.Price}");
});

await connection.StartAsync();

await Task.Delay(-1);
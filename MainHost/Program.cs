
using Core;
using Core.Interfaces;
using MainHost.CommunicationProtocols;
using MainHost.PluginLoader;
using static MainHost.CommunicationProtocols.SignalRPriceBroadcaster;

namespace MainHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IPriceChangeNotifier, SignalRPriceBroadcaster>();
            builder.Services.AddSingleton<IPriceChangeNotifier, TcpPriceServer>();
            builder.Services.AddSingleton<IPriceChangeNotifier, PluginManager>();

            builder.Services.AddSingleton<IPriceProvider, PriceProvider>();
            builder.Services.AddSingleton<IPriceBuffer>(sp => new PriceBuffer(sp.GetRequiredService<ILogger<PriceBuffer>>(), capacity: 10));
            builder.Services.AddHostedService(sp =>
            {
                var buffer = sp.GetRequiredService<IPriceBuffer>();
                var logger = sp.GetRequiredService<ILogger<PriceGeneratorService>>();
                var notifier = sp.GetRequiredService<IEnumerable<IPriceChangeNotifier>>();

                var svc = new PriceGeneratorService(
                    logger: logger,
                    initialPrices: new Dictionary<string, decimal> { ["AAPL"] = 190m, ["MSFT"] = 420m, ["GOOGL"] = 99m, ["TSLA"] = 231m, ["AMZN"] = 842m },
                    updateTime: 5000,
                    priceBuffer: buffer,
                    priceChangeNotifier: notifier
                );
                return svc;
            });
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();

            app.MapHub<PriceHub>("/hubs/prices");

            app.Run();
        }
    }
}

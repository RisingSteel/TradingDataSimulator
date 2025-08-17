
using Core;
using Core.Interfaces;
using MainHost.CommunicationProtocols;
using MainHost.PluginLoader;
using Microsoft.Extensions.Options;
using Serilog;
using static MainHost.CommunicationProtocols.SignalRPriceBroadcaster;

namespace MainHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddOptions<PriceGeneratorOptions>()
                .Bind(builder.Configuration.GetSection("PriceGeneratorOptions"))
                .ValidateDataAnnotations();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IPriceChangeNotifier, SignalRPriceBroadcaster>();
            builder.Services.AddSingleton<IPriceChangeNotifier, TcpPriceServer>();
            builder.Services.AddSingleton<IPriceChangeNotifier, PluginManager>();

            builder.Services.AddSingleton<IPriceProvider, PriceProvider>();

            builder.Services.AddSingleton<IPriceBuffer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<PriceGeneratorOptions>>().Value;
                return new PriceBuffer(sp.GetRequiredService<ILogger<PriceBuffer>>(), options.PriceBufferCapacity);
            });

            builder.Services.AddHostedService(sp =>
            {
                var buffer = sp.GetRequiredService<IPriceBuffer>();
                var logger = sp.GetRequiredService<ILogger<PriceGeneratorService>>();
                var notifier = sp.GetRequiredService<IEnumerable<IPriceChangeNotifier>>();
                var options = sp.GetRequiredService<IOptions<PriceGeneratorOptions>>().Value;

                var svc = new PriceGeneratorService(
                    logger: logger,
                    initialPrices: options.InitialPrices,
                    updateTime: options.UpdateTime,
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

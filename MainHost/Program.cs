
using Core;
using Core.Interfaces;

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
            builder.Services.AddSingleton<IPriceProvider, PriceProvider>();
            builder.Services.AddSingleton<IPriceBuffer>(sp => new PriceBuffer(sp.GetRequiredService<ILogger<PriceBuffer>>(), capacity: 10));
            builder.Services.AddHostedService(sp =>
            {
                var buffer = sp.GetRequiredService<IPriceBuffer>();
                var logger = sp.GetRequiredService<ILogger<PriceGeneratorService>>();

                var svc = new PriceGeneratorService(
                    logger: logger,
                    initialPrices: new Dictionary<string, decimal> { ["AAPL"] = 190m, ["MSFT"] = 420m, ["GOOGL"] = 99m, ["TSLA"] = 231m, ["AMZN"] = 842m },
                    updateTime: 5000,
                    priceBuffer: buffer
                );
                return svc;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();
            app.Run();
        }
    }
}

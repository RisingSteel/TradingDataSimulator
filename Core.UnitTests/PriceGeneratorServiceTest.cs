using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UnitTests
{
    public class PriceGeneratorServiceTest
    {
        [Fact]
        public async Task PriceGeneratorService_Price_Change_CheckAsync()
        {
            var logger = NullLogger<PriceGeneratorService>.Instance;
            var initialPrices = new Dictionary<string, decimal>
            {
                { "AAPL", 190.00m },
                { "TSLA", 990.00m },
                { "GOOGL", 2800.00m }
            };
            var updateTime = 1000;
            var priceBufferMock = new Moq.Mock<IPriceBuffer>();
            var priceChangeNotifierMock = Array.Empty<IPriceChangeNotifier>();
            var priceGenerator = new PriceGeneratorService(logger, initialPrices, updateTime, priceBufferMock.Object, priceChangeNotifierMock);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                await priceGenerator.StartAsync(cancellationToken);

                for (int i = 0; i < 5; i++)
                {
                    var currentPrices = priceGenerator.GetCurrentPrices();

                    await Task.Delay(updateTime + 100);

                    var lastPrices = priceGenerator.GetCurrentPrices();

                    foreach (var symbol in initialPrices.Keys)
                    {
                        var currentPrice = currentPrices[symbol];
                        var lastPrice = lastPrices[symbol];

                        decimal diffPercent = Math.Abs((currentPrice - lastPrice) / lastPrice) * 100m;

                        Assert.InRange(lastPrice, Math.Round(currentPrice * 0.98m, 2), Math.Round(currentPrice * 1.02m, 2));

                    }
                }
            }
            finally
            {
                await priceGenerator.StopAsync(cancellationToken);
            }
        }
    }
}

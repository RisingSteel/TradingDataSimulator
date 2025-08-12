using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core
{
    public class PriceGeneratorService : BackgroundService
    {
        private readonly ILogger<PriceGeneratorService> _logger;
        private readonly Dictionary<string, decimal> _initialPrices;
        private readonly int _updateTime;
        private readonly IPriceBuffer _priceBuffer;
        private Dictionary<string, decimal> _currentPrices;

        public PriceGeneratorService(ILogger<PriceGeneratorService> logger, Dictionary<string, decimal> initialPrices, int updateTime, IPriceBuffer priceBuffer)
        {
            _logger = logger;
            _updateTime = updateTime > 0 ? updateTime : throw new ArgumentOutOfRangeException(nameof(updateTime), "Update time must be greater than zero.");
            _priceBuffer = priceBuffer ?? throw new ArgumentNullException(nameof(priceBuffer));
            _initialPrices = initialPrices ?? throw new ArgumentNullException(nameof(initialPrices));
            _currentPrices = new Dictionary<string, decimal>();
            foreach (var keyValuePair in _initialPrices)
            {
                var symbol = keyValuePair.Key;
                var price = keyValuePair.Value;

                _currentPrices[symbol] = price;
                var priceTick = new PriceTick(symbol, price, DateTime.UtcNow);
                _priceBuffer.Add(priceTick);
                _logger.LogInformation("Initial price for {Symbol} : {Price}", priceTick.Symbol, priceTick.Price);
            }

            _logger.LogInformation("PriceGenerator service initialized.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var time = DateTime.UtcNow;

                foreach (var kvp in _currentPrices)
                {
                    var symbol = kvp.Key;
                    var price = kvp.Value;

                    var delta = (decimal)Random.Shared.NextDouble() * 0.04m - 0.02m;
                    var newPrice = Math.Round(price * (1 + delta), 2, MidpointRounding.AwayFromZero);

                    _currentPrices[symbol] = newPrice;
                    var priceTick = new PriceTick(symbol, newPrice, time);
                    _priceBuffer.Add(priceTick);
                    _logger.LogInformation("Generated price tick: {PriceTick}", priceTick);
                }

                await Task.Delay(_updateTime, stoppingToken);
            }
        }
    }
}

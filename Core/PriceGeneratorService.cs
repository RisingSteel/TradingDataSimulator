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
        private readonly IEnumerable<IPriceChangeNotifier> _priceChangeNotifier;

        public PriceGeneratorService(ILogger<PriceGeneratorService> logger, Dictionary<string, decimal> initialPrices, int updateTime, IPriceBuffer priceBuffer, IEnumerable<IPriceChangeNotifier> priceChangeNotifier)
        {
            _logger = logger;
            _updateTime = updateTime > 0 ? updateTime : throw new ArgumentOutOfRangeException(nameof(updateTime), "Update time must be greater than zero.");
            _priceBuffer = priceBuffer ?? throw new ArgumentNullException(nameof(priceBuffer));
            _priceChangeNotifier = priceChangeNotifier;
            _initialPrices = initialPrices ?? throw new ArgumentNullException(nameof(initialPrices));
            _logger.LogInformation("PriceGenerator service initialized.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var currentTime = DateTime.UtcNow;
            _currentPrices = new Dictionary<string, decimal>(_initialPrices);
            foreach (var kvp in _currentPrices)
            {
                var symbol = kvp.Key;
                var price = kvp.Value;

                var priceTick = new PriceTick(symbol, price, DateTime.UtcNow);
                _priceBuffer.Add(priceTick);
                _logger.LogInformation("Initial price for {Symbol} : {Price}", priceTick.Symbol, priceTick.Price);
            }



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
                    foreach (var notifier in _priceChangeNotifier)
                    {
                        _ = notifier.PriceNotifierAsync(priceTick);
                    }
                    _logger.LogInformation("Generated price tick: {PriceTick}", priceTick);
                }

                await Task.Delay(_updateTime, stoppingToken);
            }
        }

    }
}

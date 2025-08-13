using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Core
{
    public class PriceProvider : IPriceProvider
    {
        private IPriceBuffer _priceBuffer;
        private readonly ILogger<PriceProvider> _logger;
        private ConcurrentDictionary<string, PriceSlot> _buffer;

        public PriceProvider(IPriceBuffer priceBuffer, ILogger<PriceProvider> logger)
        {
            _priceBuffer = priceBuffer;
            _logger = logger;
            _buffer = _priceBuffer.GetBuffer();
        }

        public PriceTick GetLastPricebySymbol(string symbol)
        {
            if (!_buffer.TryGetValue(symbol, out var slot))
            {
                _logger.LogWarning("No price slot found for symbol: {Symbol}", symbol);
                return null;
            }
            lock (slot.Lock())
            {
                return slot._lastTick;
            }
        }

        public IEnumerable<PriceTick> GetHistoryPricebySymbol(string symbol)
        {
            if (!_buffer.TryGetValue(symbol, out var slot))
            {
                _logger.LogWarning("No price history found for symbol: {Symbol}", symbol);
                return Array.Empty<PriceTick>();
            }

            lock (slot.Lock())
            {
                return slot._slotBuffer.ToList();
            }
        }

        public IEnumerable<PriceTick> GetAllLatestPrices()
        {
            return _buffer.Values.Select(slot =>
            {
                lock (slot.Lock())
                {
                    return slot._lastTick;
                }
            });
        }
    }
}

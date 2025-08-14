using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Core
{
    public class PriceBuffer : IPriceBuffer
    {
        private readonly ILogger<PriceBuffer> _logger;
        private ConcurrentDictionary<string, PriceSlot> _buffer;
        private int _capacity;


        public PriceBuffer(ILogger<PriceBuffer> logger, int capacity)
        {
            _logger = logger;
            _capacity = capacity;
            _buffer = new ConcurrentDictionary<string, PriceSlot>();
        }
        public void Add(PriceTick priceTick)
        {

            var slot = _buffer.GetOrAdd(priceTick.Symbol, _ => new PriceSlot(_capacity));

            lock (slot.Lock())
            {
                if (slot._slotBuffer.Count >= _capacity)
                {
                    slot._slotBuffer.Dequeue();
                }
                slot._slotBuffer.Enqueue(priceTick);
                slot._lastTick = priceTick;
                _logger.LogDebug("Added price tick for {Symbol}: {Price} at {Timestamp}", priceTick.Symbol, priceTick.Price, priceTick.Timestamp);
            }
        }

        public ConcurrentDictionary<string, PriceSlot> GetBuffer()
        {
            return _buffer;
        }
    }
}

using Core.Models;
using System.Collections.Concurrent;

namespace Core.Interfaces
{
    public interface IPriceBuffer
    {
        public void Add(PriceTick priceTick);

        public ConcurrentDictionary<string, PriceSlot> GetBuffer();
    }
}

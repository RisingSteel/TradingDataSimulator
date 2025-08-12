using Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPriceBuffer
    {
        public void Add(PriceTick priceTick);

        public ConcurrentDictionary<string, PriceSlot> GetBuffer();
    }
}

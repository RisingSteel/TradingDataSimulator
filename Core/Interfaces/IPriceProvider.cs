using Core.Models;

namespace Core.Interfaces
{
    public interface IPriceProvider
    {
        public PriceTick GetLastPricebySymbol(string symbol);

        public IEnumerable<PriceTick> GetHistoryPricebySymbol(string symbol);

        public IEnumerable<PriceTick> GetAllLatestPrices();
    }
}

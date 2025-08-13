using Core.Models;

namespace Core.Interfaces
{
    public interface IPriceChangeNotifier
    {
        Task PriceNotifierAsync(PriceTick priceTick);
    }
}

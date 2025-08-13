using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace MainHost.CommunicationProtocols
{
    public class SignalRPriceBroadcaster : IPriceChangeNotifier
    {
        public sealed class PriceHub : Hub { }

        private readonly IHubContext<PriceHub> _hub;
        private readonly ILogger<SignalRPriceBroadcaster> _logger;
        public SignalRPriceBroadcaster(IHubContext<PriceHub> hub, ILogger<SignalRPriceBroadcaster> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task PriceNotifierAsync(PriceTick priceTick)
        {
            _logger.LogDebug("Broadcasting to SignalR price update for {Symbol}: {Price}", priceTick.Symbol, priceTick.Price);
            await _hub.Clients.All.SendAsync("PriceUpdated", priceTick);
        }
    }
}

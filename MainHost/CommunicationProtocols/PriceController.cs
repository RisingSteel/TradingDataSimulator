using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MainHost.CommunicationProtocols
{
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IPriceProvider _provider;
        private readonly ILogger<PriceController> _logger;
        public PriceController(ILogger<PriceController> logger, IPriceProvider provider)
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpGet("/api/prices/")]
        public IResult GetPrices(IPriceProvider provider)
        {
            _logger.LogDebug("Fetching all latest prices");
            return Results.Ok(provider.GetAllLatestPrices());
        }

        [HttpGet("/api/prices/{symbol}")]
        public IResult GetPriceBySymbol(string symbol, IPriceProvider provider)
        {
            _logger.LogDebug("Fetching last price for symbol: {Symbol}", symbol);
            var last = provider.GetLastPricebySymbol(symbol.ToUpper());
            return last is null ? Results.NotFound() : Results.Ok(last);
        }
        [HttpGet("/api/prices/{symbol}/history")]
        public IResult GetPriceHistoryBySymbol(string symbol, IPriceProvider provider)
        {
            _logger.LogDebug("Fetching price history for symbol: {Symbol}", symbol);
            var history = provider.GetHistoryPricebySymbol(symbol.ToUpper());
            return history.Count() == 0 ? Results.NotFound() : Results.Ok(history);        
        }
    }
}

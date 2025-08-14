using Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace MainHost.IntegrationTests
{
    public class PricesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public PricesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetPrices_ReturnsOkWithPrices()
        {

            var response = await _client.GetAsync("/api/prices");
            response.EnsureSuccessStatusCode();
            var prices = await response.Content.ReadFromJsonAsync<IEnumerable<PriceTick>>();
            Assert.NotNull(prices);
            Assert.NotEmpty(prices);
        }
        [Fact]
        public async Task GetPrice_BySymbol_ReturnsOkWithPrices()
        {
            var response = await _client.GetAsync("/api/prices/AAPL");
            response.EnsureSuccessStatusCode();
            var prices = await response.Content.ReadFromJsonAsync<PriceTick>();
            Assert.NotNull(prices);
        }
        [Fact]
        public async Task GetPriceHistory_BySymbol_ReturnsOkWithPrices()
        {
            var response = await _client.GetAsync("/api/prices/TSLA/history");
            response.EnsureSuccessStatusCode();
            var prices = await response.Content.ReadFromJsonAsync<IEnumerable<PriceTick>>();
            Assert.NotNull(prices);
        }
        [Fact]
        public async Task GetPrices_ByUnknownSymbol_Returns404()
        {
            var response = await _client.GetAsync("/api/prices/UNKNOWN");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetPriceHistory_ByUnknownSymbol_Returns404()
        {
            var response = await _client.GetAsync("/api/prices/UNKNOWN/history");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


    }
}
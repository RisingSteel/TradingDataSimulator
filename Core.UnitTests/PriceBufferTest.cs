using Core.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Core.UnitTests
{
    public class PriceBufferTest
    {
        [Fact]
        public void PriceBuffer_Adding_Check()
        {
            var logger = NullLogger<PriceBuffer>.Instance;

            var priceBuffer = new PriceBuffer(logger, capacity: 5);

            priceBuffer.Add(new PriceTick("AAPL", 150.00m, DateTime.Now));
            priceBuffer.Add(new PriceTick("TSLA", 990.00m, DateTime.Now));

            var buffer = priceBuffer.GetBuffer();

            Assert.Equal(2, buffer.Count());

            Assert.True(buffer.ContainsKey("AAPL") && buffer.ContainsKey("TSLA"));
        }
        [Fact]
        public void PriceBuffer_Length_Check()
        {
            var logger = NullLogger<PriceBuffer>.Instance;

            var priceBuffer = new PriceBuffer(logger, capacity: 10);


            for (int i = 0; i < 12; i++)
            {
                priceBuffer.Add(new PriceTick("AAPL", 150.00m + i, DateTime.Now));
            }

            var buffer = priceBuffer.GetBuffer();

            Assert.Equal(10, buffer["AAPL"]._slotBuffer.Count);

        }
    }
}
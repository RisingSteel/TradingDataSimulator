namespace MainHost
{
    public sealed class PriceGeneratorOptions
    {
        public Dictionary<string, decimal> InitialPrices { get; init; } = new();
        public int UpdateTime { get; init; } = 5000;
        public int PriceBufferCapacity { get; init; } = 10;

    }
}

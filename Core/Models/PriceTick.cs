namespace Core.Models
{
    public sealed record PriceTick(string Symbol, decimal Price, DateTime Timestamp);
}

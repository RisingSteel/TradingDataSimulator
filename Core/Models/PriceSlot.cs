namespace Core.Models
{
    public sealed class PriceSlot(int capacity)
    {
        private readonly object _lock = new();
        public readonly Queue<PriceTick> _slotBuffer = new Queue<PriceTick>(capacity);
        public PriceTick _lastTick { get; set; }

        public object Lock()
        {
            return _lock;
        }
    }
}

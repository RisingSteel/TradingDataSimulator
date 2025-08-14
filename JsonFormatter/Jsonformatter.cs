using DataFormaterContract;

namespace JsonFormatter
{
    public class Jsonformatter : IDataFormatter
    {
        public string FormatPrice(string symbol, decimal price, DateTime timestamp)
        {
            return $"{{\"symbol\":\"{symbol}\",\"price\":{price}\",\"timestamp\":{timestamp}}}";
        }
    }
}

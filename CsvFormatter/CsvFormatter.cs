using DataFormaterContract;

namespace CsvFormatter
{
    public class CsvFormatter : IDataFormatter
    {
        public string FormatPrice(string symbol, decimal price, DateTime timestamp)
        {
            string time = timestamp.ToString("HH:mm:ss");
            return $"{symbol},{price},{time}";
        }

    }
}

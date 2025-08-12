using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public sealed record PriceTick(string Symbol, decimal Price, DateTime TimestampUtc);
}

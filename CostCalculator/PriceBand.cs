using System;
using System.Collections.Generic;
using System.Text;

namespace CostCalculator
{
    public class PriceBand
    {
        public int QtyFrom { get; set; }
        public int? QtyTo { get; set; }
        public decimal PricePerTextMessage { get; set; }
    }
}

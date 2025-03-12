using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DomainModels
{
    public class OrderHistory
    {
        public string Photo { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string Unit { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public DateTime? ShippedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public decimal Total => Quantity * Price;
    }
}

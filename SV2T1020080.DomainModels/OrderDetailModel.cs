using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DomainModels
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}

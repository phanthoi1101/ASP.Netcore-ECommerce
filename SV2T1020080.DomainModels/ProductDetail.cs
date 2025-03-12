using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020080.DomainModels
{
    public class ProductDetail
    {
        public Product? Product { get; set; }
        public ProductAttribute? ProductAttribute { get; set; } 
        public ProductPhoto? ProductPhoto { get; set; }     

    }
}

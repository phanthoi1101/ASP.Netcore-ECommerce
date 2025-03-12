using SV21T1020080.DomainModels;

namespace SV21T1020080.Shop.Models
{
    public class ProductSearchResult:PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public int Supplier { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public required List<Product> Data { get; set; }
    }
}

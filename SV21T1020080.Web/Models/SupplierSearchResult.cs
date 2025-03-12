using SV21T1020080.DomainModels;

namespace SV21T1020080.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}

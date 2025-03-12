using SV21T1020080.DomainModels;

namespace SV21T1020080.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}

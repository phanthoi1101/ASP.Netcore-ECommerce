using SV21T1020080.DomainModels;

namespace SV21T1020080.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Categories> Data { get; set; }
    }
}

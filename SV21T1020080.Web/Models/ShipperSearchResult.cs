using SV21T1020080.DomainModels;

namespace SV21T1020080.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}

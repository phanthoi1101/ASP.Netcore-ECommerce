using SV21T1020080.DomainModels;

namespace SV21T1020080.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}

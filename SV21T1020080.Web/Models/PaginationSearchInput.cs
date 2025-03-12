namespace SV21T1020080.Web.Models
{
    /// <summary>
    /// Lưu giữ các thông tin đầu vào sử dụng cho chức năng tìm kiếm và hiển thị dưới dạng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        /// <summary>
        /// Trang cần hiển thị
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// Số dòng trên 1 trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Chuỗi chứa giá trị cần tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
    }
}

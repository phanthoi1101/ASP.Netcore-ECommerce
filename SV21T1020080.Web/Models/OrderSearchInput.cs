using SV21T1020080.Web.AppCodes;

namespace SV21T1020080.Web.Models
{
    public class OrderSearchInput
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
        /// Trạng thái hàng cần tìm
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Chuỗi chứa giá trị cần tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
        /// <summary>
        /// khoảng thời gian cần tìm(chuỗi 2 giá trị ngày có dạng dd/MM/yyy - dd/MM/yyyy)
        /// </summary>
        public string TimeRange { get; set; } = "";
        /// <summary>
        ///Lấy thời điểm bắt đầu dựa vào DateRange
        /// </summary>
        public DateTime? FromTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;
                string[] times = TimeRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[0].Trim().ToDateTime();
                    return value;
                }
                return null;
            }
            set
            {

            }
        }

        /// <summary>
        /// Lấy thời điểm kết thúc dựa vào DateRange
        /// (thời điểm kết thúc phải là cuối ngày)
        /// </summary>
        public DateTime? ToTime
        {
            get
            {

                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;
                string[] times = TimeRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[1].Trim().ToDateTime();
                    if (value.HasValue)
                        value = value.Value.AddMilliseconds(86399998); //86399999
                    return value;
                }
                return null;
            }
            set { }
        }
    }
}

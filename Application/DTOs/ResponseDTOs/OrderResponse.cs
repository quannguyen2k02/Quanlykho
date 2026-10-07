namespace Application.DTOs.ResponseDTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string Material_Code { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string EmployeeCode { get; set; }

        // Tự động tính khi serialize ra JSON
        public int Status
        {
            get
            {
                var now = ImportDate;

                if (ExpiredDate <= now) return 0; // hết hạn
                if (ExpiredDate <= now.AddMonths(3)) return 1; // dưới 3 tháng
                return 2;                                      // trên 3 tháng
            }
        }
    }
}
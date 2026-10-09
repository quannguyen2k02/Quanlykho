namespace Application.DTOs.RequestDTOs
{
    public class SlipRequest
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public string EmployeeCode { get; set; }
        public string Repository { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}

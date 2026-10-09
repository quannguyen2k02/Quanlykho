namespace Application.DTOs.ResponseDTOs
{
    public class SlipResponse
    {
        public int OrderId { get; set; }
        public string GRId { get; set; }
        public string Material_Code { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public string FromRepository { get; set; }
        public string Repository { get; set; }

        public string EmployeeCode { get; set; }
        public DateTime DateTime { get; set; }
    }
}

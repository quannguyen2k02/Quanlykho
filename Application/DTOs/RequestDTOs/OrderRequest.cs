using System.Text.Json.Serialization;

namespace Application.DTOs.RequestDTOs
{
    public class OrderRequest
    {
        public string OrderCode { get; set; }
        public string Material_Code { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public string Repository { get; set; }
        public DateTime ImportDate { get; set; }
        [JsonIgnore]
        public DateTime ExpiredDate => ImportDate.AddMonths(6);
    }
}
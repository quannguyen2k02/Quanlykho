using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Order")]
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string Material_Code { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public string Repository { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsDelete { get; set; }
        public ICollection<ExportSlip>? ExportSlips { get; set; }
    }
}
 
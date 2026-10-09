using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ExportSlip")]
    public class ExportSlip
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public string EmployeeCode { get; set; }
        public string Repository { get; set; }
        public DateTime DateTime { get; set; }
        public Order? Order { get; set; }
    }
}

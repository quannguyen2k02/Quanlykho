namespace Application.DTOs
{
    public enum ExportResultStatus
    {
        Success,
        OrderNotFound,
        InsufficientQuantity
    }

    public class ExportResult
    {
        public ExportResultStatus Status { get; set; }
        public int RemainingQuantity { get; set; }
        public string Message { get; set; }

        public static ExportResult Success(int remaining) => new()
        {
            Status = ExportResultStatus.Success,
            RemainingQuantity = remaining,
            Message = "Xuất kho thành công."
        };

        public static ExportResult NotFound() => new()
        {
            Status = ExportResultStatus.OrderNotFound,
            Message = "Không tìm thấy đơn hàng."
        };

        public static ExportResult Insufficient(int available) => new()
        {
            Status = ExportResultStatus.InsufficientQuantity,
            RemainingQuantity = available,
            Message = $"Số lượng xuất vượt quá tồn kho. Hiện còn {available}."
        };
    }
}
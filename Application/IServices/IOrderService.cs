using Application.Common;
using Application.DTOs;
using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;

namespace Application.IServices
{
    public interface IOrderService
    {
        public Task<OrderResponse> AddNewOrder(OrderRequest order);
        public Task<CursorPagedResult<dynamic>> GetOrder(int? lastId = null, int pageSize = 20);
        public Task<List<OrderResponse>> GetOrderByMaterialCode(string materialCode);
        public Task<ExportResult> ExportOrderAsync(int id, int quantity);
        public Task<List<OrderResponse>> GetOrders();

    }
}

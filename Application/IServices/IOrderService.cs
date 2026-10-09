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
        public Task<List<OrderResponse>> GetOrders(); // not include quantity = 0
        public Task<List<OrderResponse>> GetOrdersByRepositoryAndStatus(string? repository, int? status, bool? isQuantity);

        public Task<List<OrderResponse>> GetAllOrders(); // include quantity = 0
        public Task<OrderResponse> DeleteOrder(int id);

    }
}

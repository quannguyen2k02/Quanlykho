using Application.Common;
using Application.DTOs;
using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        /* ============================================================
           1) THÊM ĐƠN MỚI
           ============================================================ */
        public async Task<OrderResponse> AddNewOrder(OrderRequest order)
        {
            var o = _mapper.Map<Order>(order);
            o.IsDelete = false;

            var created = await _orderRepository.Add(o);
            return _mapper.Map<OrderResponse>(created);
        }

        /* ============================================================
           2) XOÁ MỀM
           ============================================================ */
        public async Task<OrderResponse> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetByIDAsync(id);
            if (order == null || order.IsDelete)
                return null;

            order.IsDelete = true;
            await _orderRepository.Update(order);

            return _mapper.Map<OrderResponse>(order);
        }

        /* ============================================================
           3) XUẤT KHO
           ============================================================ */
        public async Task<ExportResult> ExportOrderAsync(int id, int quantity)
        {
            // 1. Validate đầu vào
            if (quantity <= 0)
                return ExportResult.Insufficient(0);

            // 2. Tìm đơn hàng theo Id
            var o = await _orderRepository.GetByIDAsync(id);
            if (o == null)
                return ExportResult.NotFound();

            // 3. Kiểm tra số lượng tồn
            if (quantity > o.Quantity)
                return ExportResult.Insufficient(o.Quantity);
            var result = await _orderRepository.ExportOrderAsync(id, quantity);

            return ExportResult.Success(result.Quantity);
        }

        /* ============================================================
           4) LẤY TẤT CẢ (chưa xoá)
           ============================================================ */
        public async Task<List<OrderResponse>> GetAllOrders()
        {
            var orders = await _orderRepository.Find(x => x.IsDelete == false);
            return _mapper.Map<List<OrderResponse>>(orders);
        }

        /* ============================================================
           5) PHÂN TRANG (cursor) — TODO
           ============================================================ */
        public async Task<CursorPagedResult<dynamic>> GetOrder(int? lastId = null, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        /* ============================================================
           6) LẤY THEO MÃ LIỆU — chỉ lấy đơn chưa xoá & còn hạn/sắp hết
           ============================================================ */
        public async Task<List<OrderResponse>> GetOrderByMaterialCode(string materialCode)
        {
            if (string.IsNullOrWhiteSpace(materialCode))
                return new List<OrderResponse>();

            var result = await _orderRepository.GetOrdersByMaterialCode(materialCode);

            // ✅ Lọc đơn chưa xoá + bỏ đơn đã hết hạn (status != 0)
            var filtered = result
                .Where(x => x.IsDelete == false)
                .ToList();

            var response = _mapper.Map<List<OrderResponse>>(filtered);

            return response.Where(x => x.Status != 0).ToList();
        }

        /* ============================================================
           7) LẤY ĐƠN CÒN HÀNG (Quantity > 0, chưa xoá)
           ============================================================ */
        public async Task<List<OrderResponse>> GetOrders()
        {
            var result = await _orderRepository.Find(x => x.Quantity > 0 && x.IsDelete == false);
            return _mapper.Map<List<OrderResponse>>(result);
        }

        /* ============================================================
           8) ✅ FIXED — LẤY THEO REPOSITORY + STATUS + QUANTITY
           ============================================================ */
        public async Task<List<OrderResponse>> GetOrdersByRepositoryAndStatus(
            string? repository, int? status, bool? quantity)
        {
            // quantity == true  → lấy tất cả
            // quantity == false/null → chỉ lấy đơn còn hàng (Quantity > 0)
            bool onlyInStock = quantity != true;

            // Chuẩn hoá repository
            var repo = string.IsNullOrWhiteSpace(repository)
                ? null
                : repository.Trim().ToUpper();

            // ✅ Gộp IsDelete vào predicate LUÔN — bỏ biến `o` vô nghĩa
            var orders = await _orderRepository.Find(x =>
                x.IsDelete == false &&
                (repo == null || (x.Repository != null && x.Repository.ToUpper() == repo)) &&
                (!onlyInStock || x.Quantity > 0)
            );

            var response = _mapper.Map<List<OrderResponse>>(orders);

            // Filter status sau khi map (vì Status là computed trong DTO)
            if (status.HasValue)
            {
                response = response.Where(x => x.Status == status.Value).ToList();
            }

            return response;
        }
    }
}






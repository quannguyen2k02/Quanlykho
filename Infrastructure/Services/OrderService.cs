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
        public async Task<OrderResponse> AddNewOrder(OrderRequest order)
        {
            var o = _mapper.Map<Order>(order);
            return _mapper.Map<OrderResponse>(await _orderRepository.Add(o));
        }

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

        public async Task<CursorPagedResult<dynamic>> GetOrder(int? lastId = null, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderResponse>> GetOrderByMaterialCode(string materialCode)
        {
            var result = await _orderRepository.GetOrdersByMaterialCode(materialCode);
            return _mapper.Map<List<OrderResponse>>(result);
        }

        public async Task<List<OrderResponse>> GetOrders()
        {
            var result = await _orderRepository.GetAllAsync();
            return _mapper.Map<List<OrderResponse>>(result);
        }
    }
}

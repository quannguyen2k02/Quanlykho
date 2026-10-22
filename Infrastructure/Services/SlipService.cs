using Application.DTOs;
using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using Application.IRepositories;
using Application.IServices;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class SlipService : ISlipService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ISlipRepository _slipRepository;

        private readonly IMapper _mapper;
        public SlipService(IOrderRepository orderRepository, ISlipRepository slipRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _slipRepository = slipRepository;
            _mapper = mapper;
        }
        public async Task<ExportResult> AddNewSlipAsync(SlipRequest request, string employeeCode)
        {
            // 1. Validate đầu vào 
            if (request.Quantity <= 0)
                return ExportResult.Insufficient(0);

            // 2. Tìm đơn hàng theo Id
            var o = await _orderRepository.GetByIDAsync(request.OrderId);
            if (o == null)
                return ExportResult.NotFound();

            // 3. Kiểm tra số lượng tồn
            if (request.Quantity > o.Quantity)
                return ExportResult.Insufficient(o.Quantity);
            var result = await _orderRepository.ExportOrderAsync(request.OrderId, request.Quantity);
            request.DateTime = DateTime.Now;
            var slip = _mapper.Map<ExportSlip>(request);
            slip.EmployeeCode = employeeCode;
            var a = await _slipRepository.Add(slip);
            
            return ExportResult.Success(result.Quantity);
        }

        public async Task<List<SlipResponse>> GetSlips()
        {
            var result = await _slipRepository.GetDynamicSlips();
            return _mapper.Map<List<SlipResponse>>(result);
        }
    }
}

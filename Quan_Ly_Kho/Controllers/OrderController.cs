

using Application.DTOs.RequestDTOs;
using Application.IServices;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Quan_Ly_Kho_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        [Authorize()]
        public async Task<IActionResult> AddNewOrder(OrderRequest order)
        {
            var employeeCode = User.Identity?.Name;
            if (employeeCode == null)
            {
                return Unauthorized();
            }
            var result = await _orderService.AddNewOrder(order, employeeCode);
            return Ok(result);
        }
        [HttpGet("Get-order-by-materialCode")]
        [Authorize]
        public async Task<IActionResult> GetOrderByMaterialCode(string materialCode)
        {
            var result = await _orderService.GetOrderByMaterialCode(materialCode);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _orderService.GetOrders();
            return Ok(result);
        }
        [HttpPut("Export-material")]
        public async Task<IActionResult> ExportMaterial(int id, int quantity)
        {
            var result = await _orderService.ExportOrderAsync(id, quantity);
            return Ok(result);
        }
        [HttpGet("Get-orders-by-repo-and-status")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByRepositoryAndStatus(string? repository, int? status, bool? quantity)
        {
            var result = await _orderService.GetOrdersByRepositoryAndStatus(repository, status, quantity);
            return Ok(result);
        }
        [HttpDelete]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
        [HttpGet("Wheather-exist-by-location")]
        public async Task<IActionResult> IsOrderExistByLocation(string location)
        {
            var result = await _orderService.OrderExistLocation(location);
            return Ok(result);
        }
    }
}

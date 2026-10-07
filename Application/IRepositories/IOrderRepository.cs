using Application.DTOs;
using Domain.Entities;

namespace Application.IRepositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<List<Order>> GetOrdersByMaterialCode(string materialCode);
        public Task<Order> ExportOrderAsync(int id, int quantity);
    }
}

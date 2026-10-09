using Application.DTOs;
using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Order>> GetOrdersByMaterialCode(string materialCode)
        {
            var orders = await _context.Orders.Where(x => x.Material_Code == materialCode && x.Quantity > 0 && x.IsDelete == false).OrderBy(x=>x.ExpiredDate).ToListAsync();
            return orders;
        }
        public async Task<Order> ExportOrderAsync(int id, int quantity)
        {

            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            order.Quantity -= quantity;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}

using Application.DTOs.ResponseDTOs;
using Application.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SlipRepository : GenericRepository<ExportSlip>, ISlipRepository
    {
        public SlipRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<SlipResponse>> GetDynamicSlips()
        {
            return await _context.ExportSlips
        .AsNoTracking()
        .Select(s => new SlipResponse
        {
            GRId = s.Order!.OrderCode,
            Material_Code = s.Order!.Material_Code,
            Quantity = s.Quantity,
            Location = s.Location,
            EmployeeCode = s.EmployeeCode,
            DateTime = s.DateTime
        })
        .OrderByDescending(x => x.DateTime)
        .ToListAsync();
        }
    }
}
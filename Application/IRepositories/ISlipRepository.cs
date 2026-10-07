using Application.DTOs.ResponseDTOs;
using Domain.Entities;

namespace Application.IRepositories
{
    public interface ISlipRepository : IGenericRepository<ExportSlip>
    {
        public Task<List<SlipResponse>> GetDynamicSlips();
    }
}

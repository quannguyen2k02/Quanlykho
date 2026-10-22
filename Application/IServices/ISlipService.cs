using Application.DTOs;
using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;

namespace Application.IServices
{
    public interface ISlipService
    {
        public Task<ExportResult> AddNewSlipAsync(SlipRequest request, string employeeCode);
        public Task<List<SlipResponse>> GetSlips();
    }
}

using Application.DTOs.RequestDTOs;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Quan_Ly_Kho_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlipController : ControllerBase
    {
        private readonly ISlipService _iSlipService;
        public SlipController(ISlipService slipService  )
        {
            _iSlipService = slipService;
        }
        [HttpPost]
        public async Task<IActionResult> AddSlip(SlipRequest request)
        {
            var employeeCode = User.Identity?.Name;
            if(employeeCode == null)
            {
                return Unauthorized();
            }
            var result = await _iSlipService.AddNewSlipAsync(request, employeeCode);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSlips()
        {
            var result = await _iSlipService.GetSlips();
            return Ok(result);
        }

    }
}

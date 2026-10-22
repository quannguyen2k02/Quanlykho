using Application.DTOs.RequestDTOs;
using Application.IRepositories;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Quan_Ly_Kho_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountRepository _accountRepo;
        public AuthController(IAccountRepository accountRepo) => _accountRepo = accountRepo;

        [HttpPost("signup")]
        [Authorize(Roles = Roles.Administrator + "," + Roles.IDL)]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            var result = await _accountRepo.SignUpAsync(model);
            return result.Succeeded ? Ok("Đăng ký thành công") : BadRequest(result.Errors);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            var token = await _accountRepo.SignInAsync(model);
            return string.IsNullOrEmpty(token)
                ? Unauthorized("Sai tài khoản hoặc mật khẩu")
                : Ok(new { Token = token });
        }

        // Test xem thông tin bản thân
        [HttpGet("profile")]
        [Authorize] // Bắt buộc đăng nhập
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
            return Ok(new { Username = username, Roles = roles });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = Roles.Administrator)] // Chặn toàn bộ user khác
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            // Đảm bảo người gọi truyền đúng 1 trong 3 quyền quy định
            if (model.RoleName != Roles.Administrator &&
                model.RoleName != Roles.IDL &&
                model.RoleName != Roles.DL)
            {
                return BadRequest(new { Message = "Quyền không hợp lệ. Chỉ chấp nhận: Administrator, IDL, DL" });
            }

            var result = await _accountRepo.AssignRoleAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new { Message = $"Đã cấp quyền '{model.RoleName}' cho nhân viên {model.EmployeeCode} thành công!" });
        }


        [HttpGet("users")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _accountRepo.GetAllUsersAsync();
            return Ok(users);
        }

        // 2. API Reset mật khẩu (Không cần nhập mật khẩu cũ)
        [HttpPost("reset-password")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
            {
                return BadRequest(new { Message = "Mật khẩu mới phải từ 6 ký tự trở lên." });
            }

            var result = await _accountRepo.ResetPasswordAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new { Message = $"Đã reset mật khẩu cho nhân viên {model.EmployeeCode} thành công!" });
        }


        [HttpPost("remove-role")]
        [Authorize(Roles = Roles.Administrator)] // Chỉ Admin mới được gọi
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleModel model)
        {
            // Lấy mã nhân viên của người đang đăng nhập (chính là Admin đang thực hiện thao tác)
            var currentUserCode = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUserCode))
                return Unauthorized();

            // Gọi xuống Repository
            var result = await _accountRepo.RemoveRoleAsync(model, currentUserCode);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new { Message = $"Đã xóa quyền '{model.RoleName}' của nhân viên {model.EmployeeCode} thành công!" });
        }


        [HttpDelete("users/{employeeCode}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteUser(string employeeCode)
        {
            // Lấy mã của Admin đang thực hiện xóa từ Token
            var currentUserCode = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUserCode))
                return Unauthorized();

            var result = await _accountRepo.DeleteUserAsync(employeeCode, currentUserCode);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok(new { Message = $"Đã xóa tài khoản nhân viên {employeeCode} thành công!" });
        }
    }
}
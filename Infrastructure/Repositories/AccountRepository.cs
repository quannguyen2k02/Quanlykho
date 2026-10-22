using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using Application.IRepositories;
using Application.IServices;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AccountRepository(UserManager<ApplicationUser> userManager, ITokenService tokenService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AssignRoleAsync(AssignRoleModel model)
        {
            // 1. Tìm User theo mã nhân viên
            var user = await _userManager.FindByNameAsync(model.EmployeeCode);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Không tìm thấy nhân viên có mã {model.EmployeeCode}"
                });
            }

            // 2. Kiểm tra Role có tồn tại trong hệ thống không
            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"Quyền '{model.RoleName}' không tồn tại trong hệ thống"
                });
            }

            // 3. Kiểm tra xem User đã có Role này chưa
            if (await _userManager.IsInRoleAsync(user, model.RoleName))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleAlreadyAssigned",
                    Description = $"Nhân viên này đã có quyền '{model.RoleName}' rồi"
                });
            }

            // 4. Thêm Role cho User
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            // 5. (Quan trọng) Cập nhật SecurityStamp để vô hiệu hóa các Token cũ của user này
            // Nếu không cập nhật, user đó vẫn dùng token cũ (không có quyền mới) cho đến khi hết hạn.
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return result;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            // Lấy toàn bộ user từ DB (Lưu ý: Nếu dữ liệu lớn, nên phân trang)
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                // Lấy danh sách quyền của user đó
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    EmployeeCode = user.UserName!, // Identity lưu ở UserName
                    Roles = roles.ToList()
                });
            }

            return result;
        }

        public async Task<IdentityResult> RemoveRoleAsync(AssignRoleModel model, string currentUserCode)
        {
            // 1. CHẶN TỰ XÓA QUYỀN ADMIN CỦA CHÍNH MÌNH
            if (model.EmployeeCode == currentUserCode && model.RoleName == Roles.Administrator)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotRemoveOwnAdminRole",
                    Description = "Bạn không thể tự xóa quyền Administrator của chính mình để tránh khóa hệ thống!"
                });
            }

            // 2. Tìm User
            var user = await _userManager.FindByNameAsync(model.EmployeeCode);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Không tìm thấy nhân viên {model.EmployeeCode}"
                });
            }

            // 3. Kiểm tra xem User có quyền này không
            if (!await _userManager.IsInRoleAsync(user, model.RoleName))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotAssigned",
                    Description = $"Nhân viên này không có quyền '{model.RoleName}' để xóa"
                });
            }

            // 4. Thực hiện xóa quyền
            var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

            // 5. Cập nhật SecurityStamp để vô hiệu hóa các Token cũ (buộc user phải đăng nhập lại)
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            // 1. Tìm user
            var user = await _userManager.FindByNameAsync(model.EmployeeCode);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Không tìm thấy nhân viên {model.EmployeeCode}"
                });
            }

            // 2. Tạo token reset password (chuẩn của Identity)
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 3. Thực hiện đổi mật khẩu
            // Hàm này sẽ tự động kiểm tra mật khẩu mới có đúng chuẩn (độ dài, chữ, số...) không
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

            // 4. (Tuỳ chọn) Cập nhật SecurityStamp để đăng xuất user này khỏi mọi thiết bị
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return result;
        }

        public async Task<string> SignInAsync(SignInModel model)
        {
            var user = await _userManager.FindByNameAsync(model.EmployeeCode);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return string.Empty;

            return await _tokenService.GenerateTokenAsync(user);
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
        {
            var user = new ApplicationUser { UserName = model.EmployeeCode };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Mặc định user mới là IDL
                await _userManager.AddToRoleAsync(user, Roles.DL);
            }
            return result;
        }


        public async Task<IdentityResult> DeleteUserAsync(string employeeCode, string currentUserCode)
        {
            // 1. CHẶN TỰ XÓA TÀI KHOẢN CỦA CHÍNH MÌNH
            if (employeeCode == currentUserCode)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotDeleteSelf",
                    Description = "Bạn không thể tự xóa tài khoản của chính mình để tránh khóa hệ thống!"
                });
            }

            // 2. Tìm User trong DB
            var user = await _userManager.FindByNameAsync(employeeCode);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"Không tìm thấy nhân viên có mã {employeeCode}"
                });
            }

            // 3. Thực hiện xóa
            // _userManager.DeleteAsync sẽ tự động xóa user khỏi AspNetUsers, 
            // và xóa các liên kết quyền (AspNetUserRoles) của user đó.
            var result = await _userManager.DeleteAsync(user);

            return result;
        }
    }
}
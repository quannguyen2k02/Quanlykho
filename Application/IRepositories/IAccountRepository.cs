using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface IAccountRepository
    {
        Task<string> SignInAsync(SignInModel model);
        Task<IdentityResult> SignUpAsync(SignUpModel model);
        Task<IdentityResult> AssignRoleAsync(AssignRoleModel model);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
        Task<IdentityResult> RemoveRoleAsync(AssignRoleModel model, string currentUserCode);
        Task<IdentityResult> DeleteUserAsync(string employeeCode, string currentUserCode);
    }
}

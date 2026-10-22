using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RequestDTOs
{
    public class AssignRoleModel
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ResponseDTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}

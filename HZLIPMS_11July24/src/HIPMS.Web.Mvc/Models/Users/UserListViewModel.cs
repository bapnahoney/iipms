using HIPMS.Roles.Dto;
using System.Collections.Generic;

namespace HIPMS.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}

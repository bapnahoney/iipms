using HIPMS.Roles.Dto;
using System.Collections.Generic;

namespace HIPMS.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}

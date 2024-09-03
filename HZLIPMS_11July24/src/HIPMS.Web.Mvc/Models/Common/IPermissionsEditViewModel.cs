using HIPMS.Roles.Dto;
using System.Collections.Generic;

namespace HIPMS.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}
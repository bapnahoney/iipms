using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HIPMS.Roles.Dto;
using System.Threading.Tasks;

namespace HIPMS.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();

        Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input);

        Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input);
        Task<string> GetUserRoleName();
    }
}

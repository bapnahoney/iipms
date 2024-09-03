using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HIPMS.Roles.Dto;
using HIPMS.Users.Dto;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task DeActivate(EntityDto<long> user);
        Task Activate(EntityDto<long> user);
        Task<ListResultDto<RoleDto>> GetRoles();
        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
        Task<bool> SetUserSession(UserAction userAction);
        Task<bool> GetUserSession();

    }
}

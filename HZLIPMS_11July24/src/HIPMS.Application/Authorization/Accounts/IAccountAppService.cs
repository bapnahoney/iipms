using Abp.Application.Services;
using HIPMS.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace HIPMS.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}

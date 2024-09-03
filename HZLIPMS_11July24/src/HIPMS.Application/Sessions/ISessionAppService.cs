using Abp.Application.Services;
using HIPMS.Sessions.Dto;
using System.Threading.Tasks;

namespace HIPMS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

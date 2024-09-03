using Abp.Application.Services;
using HIPMS.MultiTenancy.Dto;

namespace HIPMS.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}


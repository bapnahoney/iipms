using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using HIPMS.Authorization.Users;
using HIPMS.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace HIPMS
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class HIPMSAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected HIPMSAppServiceBase()
        {
            LocalizationSourceName = HIPMSConsts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }
        //Added by honey
        protected virtual void DisposeCurrentUserAsync()
        {
            UserManager.Dispose();
            return;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}

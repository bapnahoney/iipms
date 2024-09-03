using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace HIPMS.Controllers
{
    public abstract class HIPMSControllerBase : AbpController
    {
        protected HIPMSControllerBase()
        {
            LocalizationSourceName = HIPMSConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}

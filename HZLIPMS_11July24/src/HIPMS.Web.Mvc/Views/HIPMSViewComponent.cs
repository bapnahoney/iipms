using Abp.AspNetCore.Mvc.ViewComponents;

namespace HIPMS.Web.Views
{
    public abstract class HIPMSViewComponent : AbpViewComponent
    {
        protected HIPMSViewComponent()
        {
            LocalizationSourceName = HIPMSConsts.LocalizationSourceName;
        }
    }
}

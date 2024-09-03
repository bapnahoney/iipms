using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace HIPMS.Web.Views
{
    public abstract class HIPMSRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected HIPMSRazorPage()
        {
            LocalizationSourceName = HIPMSConsts.LocalizationSourceName;
        }
    }
}

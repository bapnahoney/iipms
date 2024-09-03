using Abp.Modules;
using Abp.Reflection.Extensions;
using HIPMS.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HIPMS.Web.Startup
{
    [DependsOn(typeof(HIPMSWebCoreModule))]
    public class HIPMSWebMvcModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public HIPMSWebMvcModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<HIPMSNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HIPMSWebMvcModule).GetAssembly());
        }
    }
}

using Abp.Modules;
using Abp.Reflection.Extensions;
using HIPMS.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HIPMS.Web.Host.Startup
{
    [DependsOn(
       typeof(HIPMSWebCoreModule))]
    public class HIPMSWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public HIPMSWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HIPMSWebHostModule).GetAssembly());
        }
    }
}

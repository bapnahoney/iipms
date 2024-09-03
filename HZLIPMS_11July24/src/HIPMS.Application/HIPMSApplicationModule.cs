using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HIPMS.Authorization;

namespace HIPMS
{
    [DependsOn(
        typeof(HIPMSCoreModule),
        typeof(AbpAutoMapperModule))]
    public class HIPMSApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<HIPMSAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(HIPMSApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}

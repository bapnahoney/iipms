using Abp.Authorization;
using Abp.Runtime.Session;
using HIPMS.Configuration.Dto;
using System.Threading.Tasks;

namespace HIPMS.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : HIPMSAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}

using HIPMS.Configuration.Dto;
using System.Threading.Tasks;

namespace HIPMS.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}

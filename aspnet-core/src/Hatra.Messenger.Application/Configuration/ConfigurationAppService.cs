using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Runtime.Session;
using Hatra.Messenger.Configuration.Dto;

namespace Hatra.Messenger.Configuration
{
    [AbpAuthorize]
    [RemoteService(false)]
    public class ConfigurationAppService : MessengerAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}

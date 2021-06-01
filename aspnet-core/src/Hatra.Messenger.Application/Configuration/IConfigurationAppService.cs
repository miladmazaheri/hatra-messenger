using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Configuration.Dto;

namespace Hatra.Messenger.Configuration
{
    [RemoteService(false)]
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}

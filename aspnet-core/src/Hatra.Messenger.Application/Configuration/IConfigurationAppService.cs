using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Configuration.Dto;

namespace Hatra.Messenger.Configuration
{
    
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}

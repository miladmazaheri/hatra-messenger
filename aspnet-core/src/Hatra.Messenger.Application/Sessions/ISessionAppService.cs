using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Sessions.Dto;

namespace Hatra.Messenger.Sessions
{
    
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

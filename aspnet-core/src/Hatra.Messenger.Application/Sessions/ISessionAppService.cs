using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Sessions.Dto;

namespace Hatra.Messenger.Sessions
{
    [RemoteService(false)]
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

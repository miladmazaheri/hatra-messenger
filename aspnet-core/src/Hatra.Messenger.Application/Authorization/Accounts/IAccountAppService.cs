using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Authorization.Accounts.Dto;

namespace Hatra.Messenger.Authorization.Accounts
{
    
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}

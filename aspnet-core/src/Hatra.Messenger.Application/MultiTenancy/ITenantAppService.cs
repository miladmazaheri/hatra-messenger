using Abp.Application.Services;
using Hatra.Messenger.MultiTenancy.Dto;

namespace Hatra.Messenger.MultiTenancy
{
    [RemoteService(false)]
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}


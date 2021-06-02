using Abp.Application.Services;
using Hatra.Messenger.MultiTenancy.Dto;

namespace Hatra.Messenger.MultiTenancy
{
    
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}


using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Hatra.Messenger.MultiTenancy;

namespace Hatra.Messenger.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}

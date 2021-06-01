using Abp.Application.Services.Dto;

namespace Hatra.Messenger.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}


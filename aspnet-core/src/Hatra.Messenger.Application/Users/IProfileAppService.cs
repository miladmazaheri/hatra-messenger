using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Users.Dto;

namespace Hatra.Messenger.Users
{
    public interface IProfileAppService : IApplicationService
    {
        Task<UserProfileDto> GetAsync();
        Task UpdateAsync(UpdateUserProfileDto input);
    }
}

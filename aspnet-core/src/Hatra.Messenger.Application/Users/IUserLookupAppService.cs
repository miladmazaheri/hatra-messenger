using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Common.Users;

namespace Hatra.Messenger.Users
{
    public interface IUserLookupAppService :IApplicationService
    {
        Task<List<UserInfoDto>> GetAllByPhoneListAsync(List<string> phones);
    }
}

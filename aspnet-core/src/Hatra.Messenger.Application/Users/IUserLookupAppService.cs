using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Common.Users;
using JetBrains.Annotations;

namespace Hatra.Messenger.Users
{
    public interface IUserLookupAppService :IApplicationService
    {
        Task<List<UserInfoDto>> GetContactsAsync([CanBeNull] List<string> phones, [CanBeNull] List<string> usernames);
    }
}

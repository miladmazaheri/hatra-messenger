using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Hatra.Messenger.Common.Users;
using Hatra.Messenger.EntityFrameworkCore.Repositories;

namespace Hatra.Messenger.Users
{
    [AbpAuthorize]
    [RemoteService(false)]
    public class UserLookupAppService :ApplicationService,IUserLookupAppService
    {
        private readonly IUserRepository _userRepository;

        public UserLookupAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<List<UserInfoDto>> GetAllByPhoneListAsync(List<string> phones)
        {
            return _userRepository.GetAllByPhoneListAsync(phones);
        }
    }
}

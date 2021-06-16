using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Runtime.Security;
using Hatra.Messenger.Authorization.Roles;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Users.Dto;
using Microsoft.AspNetCore.Http;

namespace Hatra.Messenger.Users
{
    [AbpAuthorize]
    public class ProfileAppService : MessengerAppServiceBase, IProfileAppService
    {
        private readonly UserManager _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProfileAppService(UserManager userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public virtual async Task<UserProfileDto> GetAsync()
        {
            var userId = _contextAccessor.HttpContext?.User.Identity.GetUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException();
            }
            var user = await _userManager.GetUserByIdAsync(userId.Value);
            return MapToEntityDto(user);
        }
        public async Task UpdateAsync(UpdateUserProfileDto input)
        {
            var userId = _contextAccessor.HttpContext?.User.Identity.GetUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException();
            }
            var user = await _userManager.GetUserByIdAsync(userId.Value);
            input.ApplyCorrectYeKe();
            MapToEntity(input, user);
            CheckErrors(await _userManager.UpdateAsync(user));
        }

        private void MapToEntity(UpdateUserProfileDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }
        private UserProfileDto MapToEntityDto(User user)
        {
            return ObjectMapper.Map<UserProfileDto>(user);
        }
    }
}

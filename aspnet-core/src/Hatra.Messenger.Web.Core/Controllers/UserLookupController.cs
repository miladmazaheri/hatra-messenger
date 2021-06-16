using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Hatra.Messenger.Authorization;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Models.TokenAuth;
using Hatra.Messenger.Users.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hatra.Messenger.Controllers
{
    [Route("api/[controller]/[action]")]
    [AbpAuthorize]
    public class UserLookupController : MessengerControllerBase
    {
        private readonly UserManager _userManager;

        public UserLookupController(UserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<UserInfoDto>> GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest();

            var user = await _userManager.GetByUsernameAsync(username);
            if (user == null) return NotFound();

            return new ActionResult<UserInfoDto>(new UserInfoDto
            {
                Id = user.Id,
                Status = string.Empty,
                FullName = user.FullName,
                AvatarAddress = string.Empty,
                Username = user.UserName
            });
        }

        [HttpGet]
        public async Task<ActionResult<UserInfoDto>> GetById(long id)
        {
            var user = await _userManager.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return new ActionResult<UserInfoDto>(new UserInfoDto
            {
                Id = user.Id,
                Status = string.Empty,
                FullName = user.FullName,
                AvatarAddress = string.Empty,
                Username = user.UserName
            });
        }
    }
}

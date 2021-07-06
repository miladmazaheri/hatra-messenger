using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Extensions;
using DNTPersianUtils.Core;
using Hatra.Messenger.Authorization;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Common.Users;
using Hatra.Messenger.Models.TokenAuth;
using Hatra.Messenger.Models.Users;
using Hatra.Messenger.Users;
using Hatra.Messenger.Users.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hatra.Messenger.Controllers
{
    [Route("api/[controller]/[action]")]
    [AbpAuthorize]
    public class UserLookupController : MessengerControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IUserLookupAppService _userLookupAppService;
        public UserLookupController(UserManager userManager, IUserLookupAppService userLookupAppService)
        {
            _userManager = userManager;
            _userLookupAppService = userLookupAppService;
        }

        [HttpGet]
        public async Task<ActionResult<UserInfoDto>> GetByUsernameOrPhone(string usernameOrPhone)
        {
            if (string.IsNullOrWhiteSpace(usernameOrPhone)) return BadRequest();
            User user;
            if (usernameOrPhone.IsValidIranianMobileNumber())
            {
                user = await _userManager.FindByPhoneNumber(usernameOrPhone);
                if (user != null)
                {
                    return new ActionResult<UserInfoDto>(new UserInfoDto
                    {
                        Id = user.Id,
                        Status = user.Status,
                        FullName = user.FullName,
                        AvatarAddress = user.AvatarAddress,
                        Username = user.UserName,
                        PhoneNumber = user.PhoneNumber
                    });
                }
            }
            else
            {
                user = await _userManager.GetByUsernameAsync(usernameOrPhone);
                if (user != null)
                {
                    return new ActionResult<UserInfoDto>(new UserInfoDto
                    {
                        Id = user.Id,
                        Status = user.Status,
                        FullName = user.FullName,
                        AvatarAddress = user.AvatarAddress,
                        Username = user.UserName
                    });
                }
            }

            return NotFound();

            
        }

        [HttpGet]
        public async Task<ActionResult<UserInfoDto>> GetById(long id)
        {
            var user = await _userManager.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return new ActionResult<UserInfoDto>(new UserInfoDto
            {
                Id = user.Id,
                Status = user.Status,
                FullName = user.FullName,
                AvatarAddress = user.AvatarAddress,
                Username = user.UserName
            });
        }

        [HttpPost]
        public async Task<ActionResult<List<UserInfoDto>>> GetContacts([FromBody] GetContactsModel model)
        {
            if (model == null || ((model.Phones == null || model.Phones.Count == 0) && (model.Usernames == null || model.Usernames.Count == 0))) return BadRequest();
            var phones = model.Phones?.Where(x => x.IsValidIranianMobileNumber()).ToList() ?? new List<string>();
            var usernames = model.Usernames?.Where(x => !x.IsNullOrWhiteSpace()).ToList() ?? new List<string>();
            return new ActionResult<List<UserInfoDto>>(await _userLookupAppService.GetContactsAsync(phones, usernames));
        }
    }
}

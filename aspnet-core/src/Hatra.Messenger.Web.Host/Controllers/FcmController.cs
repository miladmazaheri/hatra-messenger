using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Security;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Controllers;
using Hatra.Messenger.Web.Host.Hubs;

namespace Hatra.Messenger.Web.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [AbpAuthorize]
    public class FcmController : MessengerControllerBase
    {
        private readonly IFcmTokenService _fcmTokenService;

        public FcmController(IFcmTokenService fcmTokenService)
        {
            _fcmTokenService = fcmTokenService;
        }

        [HttpPost]
        public async Task Update([FromQuery]string token)
        {
            var userId = HttpContext.User.Identity.GetUserId().Value;
            ChatHub.UserFcmTokens.AddOrUpdate(userId, x => token, (x, t) => token);
            await _fcmTokenService.InsertOrUpdateAsync(userId, token);
        }
    }
}

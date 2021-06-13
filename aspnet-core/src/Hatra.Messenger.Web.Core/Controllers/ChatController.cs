using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Security;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Common.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Hatra.Messenger.Controllers
{

    [Route("api/[controller]/[action]")]
    [AbpAuthorize]
    public class ChatController : MessengerControllerBase
    {
        private readonly IChatAppService _chatAppService;

        public ChatController(IChatAppService chatAppService)
        {
            _chatAppService = chatAppService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatListItemDto>>> GetChatHistory()
        {
            var userId = User.Identity.GetUserId().Value;
            return new ActionResult<List<ChatListItemDto>>(await _chatAppService.GetChatHistoryAsync(userId));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Security;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
using Hatra.Messenger.Models.Chat;
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
        public async Task<ActionResult<List<ChatListItemWithLastContentDto>>> GetChatHistory()
        {
            var userId = User.Identity.GetUserId().Value;
            return new ActionResult<List<ChatListItemWithLastContentDto>>(await _chatAppService.GetChatHistoryAsync(userId));
        }
        [HttpGet]
        public async Task<ActionResult<List<ChatContentDto>>> GetChatContent(ChatContentRequestDto model)
        {
            if (!await _chatAppService.CanGetContentAsync(User.Identity.GetUserId().Value, model.ChatId))
            {
                return Unauthorized("You Can Not Access Requested Chat");
            }
            return new ActionResult<List<ChatContentDto>>(await _chatAppService.GetChatContentAsync(model));
        }
        [HttpPost]
        public async Task<ActionResult<ChatListItemDto>> StartPrivateChat([FromBody] StartPrivateChatModel model)
        {
            var userId = User.Identity.GetUserId().Value;
            return new ActionResult<ChatListItemDto>(await _chatAppService.StartPrivateChatAsync(userId, model.ReceiverId));
        }

        //TODO: Delete This Method
        [HttpDelete]
        public Task ClearAll()
        {
            return _chatAppService.ClearAllAsync();
        }
    }
}

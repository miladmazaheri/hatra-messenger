using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Security;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
using Hatra.Messenger.Controllers;
using Hatra.Messenger.Models.Chat;
using Hatra.Messenger.Web.Host.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Controllers
{

    [Route("api/[controller]/[action]")]
    [AbpAuthorize]
    public class ChatController : MessengerControllerBase
    {
        private readonly IChatAppService _chatAppService;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(IChatAppService chatAppService, IHubContext<ChatHub> hubContext)
        {
            _chatAppService = chatAppService;
            _hubContext = hubContext;
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

        [HttpDelete]
        public async Task<ActionResult> DeleteMessage(DeleteChatContentDto model)
        {
            var userId = User.Identity.GetUserId().Value;
            if (!await _chatAppService.CanGetContentAsync(userId, model.ChatId))
            {
                return Unauthorized("You Can Not Access Requested Chat");
            }
            await _chatAppService.DeleteChatContentAsync(userId, model.MessageId);


            if (ChatHub.OnlineUsers.TryGetValue(model.ReceiverId, out var connectionId))
                await _hubContext.Clients.Clients(connectionId).PushDeleteMessageAsync(model.ChatId, model.MessageId);


            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteChat(DeleteChatDto model)
        {
            var userId = User.Identity.GetUserId().Value;
            if (!await _chatAppService.CanGetContentAsync(userId, model.ChatId))
            {
                return Unauthorized("You Can Not Access Requested Chat");
            }
            await _chatAppService.DeleteParticipantChatAsync(userId, model.ChatId);


            if (ChatHub.OnlineUsers.TryGetValue(model.ReceiverId, out var connectionId))
                await _hubContext.Clients.Clients(connectionId).PushDeleteChatAsync(model.ChatId);

            return Ok();
        }

        //TODO: Delete This Method
        [HttpDelete]
        public Task ClearAll()
        {
            return _chatAppService.ClearAllAsync();
        }
    }
}

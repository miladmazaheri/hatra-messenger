using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    //[AbpAuthorize]
    public class ChatHub : Hub, ITransientDependency
    {
        public static readonly ConcurrentDictionary<long, string> OnlineUsers = new ConcurrentDictionary<long, string>();
        protected IChatAppService ChatService { get; }
        public ChatHub(IChatAppService chatService)
        {
            ChatService = chatService;
        }
        public async Task SendMessage(string message)
        {
            var username = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "unknown";
            await Clients.All.SendAsync("getMessage", JsonSerializer.Serialize(new MessageModel(username, message)));
        }
        public async Task SendPrivateMessage(long receiverId, Guid chatId, string message)
        {
            var messageModel = JsonSerializer.Deserialize<ReceivedMessageDto>(message);
            if (messageModel != null && messageModel.IsValid())
            {
                var content = new ChatContentDto(chatId, Context.GetUserId(), DateTime.Now, messageModel);
                await ChatService.InsertContentAsync(content);

                if (OnlineUsers.TryGetValue(receiverId, out var connectionId))
                {
                    await Clients.Client(connectionId).SendPrivateMessageAsync(JsonSerializer.Serialize(content));
                }
                else
                {
                    //TODO:Call FireBase API
                }
            }
        }


        public override async Task OnConnectedAsync()
        {
            OnlineUsers.GetOrAdd(Context.GetUserId(), x => Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUsers.TryRemove(Context.GetUserId(), out var res);
            await base.OnDisconnectedAsync(exception);
        }
    }

    public class MessageModel
    {
        public string Sender { get; set; }
        public string Content { get; set; }

        public MessageModel(string sender, string content)
        {
            Sender = sender;
            Content = content;
        }

        public MessageModel()
        {

        }
    }
}

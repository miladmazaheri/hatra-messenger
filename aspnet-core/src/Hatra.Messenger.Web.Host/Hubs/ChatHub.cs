using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using DNTPersianUtils.Core;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    //[AbpAuthorize]
    public class ChatHub : Hub, ITransientDependency
    {
        public static readonly ConcurrentDictionary<long, List<string>> OnlineUsers = new ConcurrentDictionary<long, List<string>>();
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
                messageModel.ApplyCorrectYeKe();
                var content = new ChatContentDto(chatId, Context.GetUserId(), DateTime.Now, messageModel);
                await ChatService.InsertContentAsync(content);

                if (OnlineUsers.TryGetValue(receiverId, out var connectionIds))
                {
                    await Clients.Clients(connectionIds).SendPrivateMessageAsync(JsonSerializer.Serialize(content, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
                }
                else
                {
                    //TODO:Call FireBase API
                }
            }
        }


        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetUserId();
            if (OnlineUsers.TryGetValue(userId, out var connectionIds))
            {
                if (connectionIds.All(x => x != Context.ConnectionId))
                {
                    connectionIds.Add(Context.ConnectionId);
                }
            }
            else
            {
                var lst = new List<string> { Context.ConnectionId };
                OnlineUsers.GetOrAdd(userId, x => lst);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.GetUserId();
            if (OnlineUsers.TryGetValue(userId, out var connectionIds))
            {
                connectionIds.Remove(Context.ConnectionId);
                if (connectionIds.Count == 0)
                {
                    OnlineUsers.TryRemove(userId, out _);
                }
            }

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

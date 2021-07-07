using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    //[AbpAuthorize]
    public class ChatHub : Hub, ITransientDependency
    {
        public static readonly ConcurrentDictionary<long, List<string>> OnlineUsers = new ConcurrentDictionary<long, List<string>>();
        public static ConcurrentDictionary<long, string> UserFcmTokens = new ConcurrentDictionary<long, string>();
        protected IChatAppService ChatService { get; }
        protected IWebHostEnvironment HostEnvironment;
        protected static FirebaseMessaging FireBaseMessaging;

        public ChatHub(IChatAppService chatService, IWebHostEnvironment hostEnvironment)
        {
            ChatService = chatService;
            HostEnvironment = hostEnvironment;
            if (FirebaseApp.DefaultInstance == null)
            {
                var app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(Path.Combine(HostEnvironment.ContentRootPath, "hatra-chat-firebase-adminsdk-b0a02-594ba3d9ba.json")),
                });
                FireBaseMessaging = FirebaseMessaging.GetMessaging(app);
            }

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
                    if (UserFcmTokens.TryGetValue(receiverId, out var fcmToken))
                    {
                        await SendFcmMessage(fcmToken, content);
                    }
                }
            }
        }
        private async Task SendFcmMessage(string token, ChatContentDto messageModel)
        {
            try
            {
                var senderNameClaim = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);

                var dataDictionary = new Dictionary<string, string> { { "result", JsonSerializer.Serialize(messageModel) } };
                _ = await FireBaseMessaging.SendAsync(new Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Body = messageModel.Text,
                        ImageUrl = messageModel.ThumbnailAddress,
                        Title = senderNameClaim?.Value ?? string.Empty
                    },
                    Data = dataDictionary
                });
            }
            catch (Exception)
            {
                // ignored
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

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
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    [AbpAuthorize]
    public class ChatHub : Hub, ITransientDependency
    {
        public static readonly ConcurrentDictionary<long, string> OnlineUsers = new ConcurrentDictionary<long, string>();

        public ChatHub()
        {
        }

        public async Task SendMessage(string message)
        {
            var username = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "unknown";
            await Clients.All.SendAsync("getMessage", JsonSerializer.Serialize(new MessageModel(username, message)));
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

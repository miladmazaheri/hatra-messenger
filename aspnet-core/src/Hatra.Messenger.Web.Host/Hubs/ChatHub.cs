using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public ILogger Logger { get; set; }

        public ChatHub()
        {
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
        }

        public async Task SendMessage(string message)
        {
            var username = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "unknown";
            await Clients.All.SendAsync("getMessage", JsonSerializer.Serialize(new MessageModel(username, message)));
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Logger.Debug("A client connected to MyChatHub: " + Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Logger.Debug("A client disconnected from MyChatHub: " + Context.ConnectionId);
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

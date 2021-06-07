using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    public class ChatHub: Hub, ITransientDependency
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
            await Clients.Others.SendAsync("getMessage", string.Format("User {0}: {1}", AbpSession.UserId, message));
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
}

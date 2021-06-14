using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Hubs
{
    public static class ChatHubExtensions
    {
        private static string UploadProgressPercent = "UploadProgressPercent";
        private static string PrivateMessage = "PrivateMessage";


        public static async Task PushUploadProgressPercentAsync(this IClientProxy proxy, Guid mediaId, int percent)
        {
            await proxy.SendAsync(UploadProgressPercent, mediaId, percent);
        }
        public static async Task SendPrivateMessageAsync(this IClientProxy proxy,string message)
        {
            await proxy.SendAsync(PrivateMessage, message);
        }
    }
}

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
        private static string DeleteMessage = "DeleteMessage";
        private static string DeleteChat = "DeleteChat";


        public static async Task PushUploadProgressPercentAsync(this IClientProxy proxy, string uploadKey, int percent)
        {
            await proxy.SendAsync(UploadProgressPercent, uploadKey, percent);
        }
        public static async Task PushDeleteMessageAsync(this IClientProxy proxy, Guid chatId,List<Guid> messageIds)
        {
            await proxy.SendAsync(DeleteMessage, chatId, messageIds);
        }
        public static async Task PushDeleteChatAsync(this IClientProxy proxy, Guid chatId)
        {
            await proxy.SendAsync(DeleteChat, chatId);
        }
        public static async Task SendPrivateMessageAsync(this IClientProxy proxy,string message)
        {
            await proxy.SendAsync(PrivateMessage, message);
        }
    }
}

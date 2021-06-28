using System.Collections.Concurrent;
using Abp.AspNetCore.SignalR;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Hatra.Messenger.Chat;
using Hatra.Messenger.Chats.Entities;
using Hatra.Messenger.Configuration;
using Hatra.Messenger.Web.Host.Hubs;

namespace Hatra.Messenger.Web.Host.Startup
{
    [DependsOn(
       typeof(MessengerWebCoreModule), typeof(AbpAspNetCoreSignalRModule))]
    public class MessengerWebHostModule : AbpModule
    {
        private readonly IIocResolver _iocResolver;

        public MessengerWebHostModule(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessengerWebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            var fcmService = _iocResolver.Resolve<IFcmTokenService>();
            ChatHub.UserFcmTokens = new ConcurrentDictionary<long, string>(fcmService.GetAllAsDictionaryAsync().Result);

        }
    }
}

using Abp.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Hatra.Messenger.Configuration;

namespace Hatra.Messenger.Web.Host.Startup
{
    [DependsOn(
       typeof(MessengerWebCoreModule),typeof(AbpAspNetCoreSignalRModule))]
    public class MessengerWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public MessengerWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessengerWebHostModule).GetAssembly());
        }
    }
}

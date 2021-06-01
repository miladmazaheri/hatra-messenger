using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Hatra.Messenger.EntityFrameworkCore;
using Hatra.Messenger.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Hatra.Messenger.Web.Tests
{
    [DependsOn(
        typeof(MessengerWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class MessengerWebTestModule : AbpModule
    {
        public MessengerWebTestModule(MessengerEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MessengerWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(MessengerWebMvcModule).Assembly);
        }
    }
}
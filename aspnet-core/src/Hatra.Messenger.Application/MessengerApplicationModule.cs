using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Hatra.Messenger.Authorization;

namespace Hatra.Messenger
{
    [DependsOn(
        typeof(MessengerCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class MessengerApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<MessengerAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(MessengerApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}

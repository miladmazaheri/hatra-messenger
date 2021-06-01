using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Hatra.Messenger.Authorization;
using Hatra.Messenger.Authorization.Passwordless;
using Hatra.Messenger.Authorization.Roles;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Editions;
using Hatra.Messenger.MultiTenancy;

namespace Hatra.Messenger.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            var builder =  services.AddAbpIdentity<Tenant, User, Role>()
                .AddAbpTenantManager<TenantManager>()
                .AddAbpUserManager<UserManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpEditionManager<EditionManager>()
                .AddAbpUserStore<UserStore>()
                .AddAbpRoleStore<RoleStore>()
                .AddAbpLogInManager<LogInManager>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders()
                .AddPasswordlessLoginProvider();;

            
            return builder;
        }
    }
}

using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using Hatra.Messenger.Authorization.Roles;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.MultiTenancy;

namespace Hatra.Messenger.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        public LogInManager(
            UserManager userManager, 
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager, 
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository, 
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher, 
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory) 
            : base(
                  userManager, 
                  multiTenancyConfig,
                  tenantRepository, 
                  unitOfWorkManager, 
                  settingManager, 
                  userLoginAttemptRepository, 
                  userManagementConfig, 
                  iocResolver, 
                  passwordHasher, 
                  roleManager, 
                  claimsPrincipalFactory)
        {
        }

        public virtual async Task SaveSuccessfulLoginAttemptAsync(long userId,string username)
        {
             using var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress);
             var tenantId = 1;
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var loginAttempt = new UserLoginAttempt
                {
                    TenantId = tenantId,
                    TenancyName = "Default",
                    UserId = userId,
                    UserNameOrEmailAddress = username,
                    Result = AbpLoginResultType.Success,
                    BrowserInfo = ClientInfoProvider.BrowserInfo,
                    ClientIpAddress = ClientInfoProvider.ClientIpAddress,
                    ClientName = ClientInfoProvider.ComputerName,
                };

                await UserLoginAttemptRepository.InsertAsync(loginAttempt);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                await uow.CompleteAsync();
            }
        }
    }
}

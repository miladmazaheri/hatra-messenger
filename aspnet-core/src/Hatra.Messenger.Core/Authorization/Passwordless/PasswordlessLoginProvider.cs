using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hatra.Messenger.Authorization.Passwordless
{
    public class PasswordlessLoginProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser>
        where TUser : class
    {
       
        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(true);
        }

        public override async  Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var userId = await manager.GetUserIdAsync(user);

            return "PasswordlessLogin:" + purpose + ":" + userId;
        }
    }

    
}

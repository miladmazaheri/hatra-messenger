using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hatra.Messenger.Authorization.Passwordless
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddPasswordlessLoginProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var totpProvider = typeof(PasswordlessLoginProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider("PasswordlessLoginProvider", totpProvider);
        }
    }
}

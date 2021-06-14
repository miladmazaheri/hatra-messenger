using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace Hatra.Messenger.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public string AvatarAddress{ get; set; }
        public string Status { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        
        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}

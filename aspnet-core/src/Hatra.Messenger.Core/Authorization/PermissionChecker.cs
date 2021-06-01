using Abp.Authorization;
using Hatra.Messenger.Authorization.Roles;
using Hatra.Messenger.Authorization.Users;

namespace Hatra.Messenger.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}

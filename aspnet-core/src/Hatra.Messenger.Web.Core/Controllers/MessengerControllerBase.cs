using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Hatra.Messenger.Controllers
{
    public abstract class MessengerControllerBase: AbpController
    {
        protected MessengerControllerBase()
        {
            LocalizationSourceName = MessengerConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace Hatra.Messenger.SMS
{
    [RemoteService(false)]
    public interface ISmsAppService:IApplicationService
    {
        Task<bool> SendVerifyCodeAsync(string receptor, string token);
    }
}

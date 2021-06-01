using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Kavenegar;
using Kavenegar.Core.Models.Enums;

namespace Hatra.Messenger.SMS
{
    [RemoteService(false)]
    public class SmsAppService : MessengerAppServiceBase, ISmsAppService
    {
        private readonly KavenegarSetting _kavenegarSetting;
        public SmsAppService(KavenegarSetting kavenegarSetting)
        {
            _kavenegarSetting = kavenegarSetting;
        }

        public async Task<bool> SendVerifyCodeAsync(string receptor, string token)
        {
            var res = await new KavenegarApi(_kavenegarSetting.ApiKey).VerifyLookup(receptor, token, "verify", VerifyLookupType.Sms);
            return res.Status == 0;
        }
    }
}

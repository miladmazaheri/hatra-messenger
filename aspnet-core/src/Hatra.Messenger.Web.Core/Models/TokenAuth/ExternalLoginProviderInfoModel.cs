using Abp.AutoMapper;
using Hatra.Messenger.Authentication.External;

namespace Hatra.Messenger.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}

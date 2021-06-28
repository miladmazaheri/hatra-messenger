using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace Hatra.Messenger.Chat
{
    public interface IFcmTokenService:IApplicationService
    {
        Task InsertOrUpdateAsync(long id, string token);
        Task<Dictionary<long, string>> GetAllAsDictionaryAsync();
    }
}

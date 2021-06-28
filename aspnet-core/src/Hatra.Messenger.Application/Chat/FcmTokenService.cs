using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Hatra.Messenger.Chats.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hatra.Messenger.Chat
{
    public class FcmTokenService : ApplicationService, IFcmTokenService
    {
        private readonly IRepository<FcmToken, long> _tokenRepository;

        public FcmTokenService(IRepository<FcmToken, long> tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task InsertOrUpdateAsync(long id, string token)
        {
            var item = await _tokenRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                await _tokenRepository.InsertAsync(new FcmToken
                {
                    Id = id,
                    Token = token
                });
            }
            else
            {
                item.Token = token;
                await _tokenRepository.UpdateAsync(item);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public Task<Dictionary<long, string>> GetAllAsDictionaryAsync()
        {
            return _tokenRepository.GetAll().ToDictionaryAsync(x => x.Id, x => x.Token);
        }
    }
}

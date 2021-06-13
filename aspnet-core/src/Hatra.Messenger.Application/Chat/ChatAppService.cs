using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.EntityFrameworkCore.Repositories;

namespace Hatra.Messenger.Chat
{
    [RemoteService(false)]
    public class ChatAppService : MessengerAppServiceBase, IChatAppService
    {
        private readonly IChatRepository _chatRepository;

        public ChatAppService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<ChatListItemDto>> GetChatHistoryAsync(long userId)
        {
            return await _chatRepository.GetChatHistoryAsync(userId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
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

        public Task<List<ChatListItemWithLastContentDto>> GetChatHistoryAsync(long userId)
        {
            return _chatRepository.GetChatHistoryAsync(userId);
        }

        public Task<ChatListItemDto> StartPrivateChatAsync(long userId, long userReceiverId)
        {
            return _chatRepository.StartPrivateChatAsync(userId, userReceiverId);
        }

        public Task InsertContentAsync(ChatContentDto model)
        {
            return _chatRepository.InsertContentAsync(model);
        }

        public Task ClearAllAsync()
        {
            return _chatRepository.ClearAllAsync();
        }

        public Task<List<ChatContentDto>> GetChatContentAsync(ChatContentRequestDto model)
        {
            return _chatRepository.GetChatContentAsync(model);
        }

        public Task<bool> CanGetContentAsync(long userId, Guid chatId)
        {
            return _chatRepository.CanGetContentAsync(userId, chatId);
        }

        public Task DeleteChatContentAsync(long userId, List<Guid> messageIds)
        {
            return _chatRepository.DeleteChatContentAsync(userId, messageIds);
        }

        public Task DeleteParticipantChatAsync(long userId, Guid chatId)
        {
            return _chatRepository.DeleteParticipantChatAsync(userId, chatId);
        }

        public Task MessageAckAsync(Guid messageId)
        {
            return _chatRepository.MessageAckAsync(messageId);
        }

        public Task ViewAckAsync(long userId, Guid chatId)
        {
             return _chatRepository.ViewAckAsync(userId, chatId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Abp;
using Abp.Data;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Chats.Entities;
using Hatra.Messenger.Chats.Enums;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Common.DataTransferObjects.Chat;
using Hatra.Messenger.Tools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    public interface IChatRepository : IRepository<Chat, Guid>
    {
        Task<List<ChatListItemWithLastContentDto>> GetChatHistoryAsync(long userId);
        Task<ChatListItemDto> StartPrivateChatAsync(long userId, long userReceiverId);
        Task<List<ChatContentDto>> GetChatContentAsync(ChatContentRequestDto model);
        Task InsertContentAsync(ChatContentDto model);
        Task ClearAllAsync();
        Task<bool> CanGetContentAsync(long userId, Guid chatId);
    }
    public class ChatRepository : MessengerRepositoryBase<Chat, Guid>, IChatRepository
    {
        private readonly IRepository<User, long> _userRepository;

        public ChatRepository(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, IRepository<User, long> userRepository) : base(dbContextProvider, transactionProvider)
        {
            _userRepository = userRepository;
        }

        public async Task<List<ChatListItemWithLastContentDto>> GetChatHistoryAsync(long userId)
        {
            await EnsureConnectionOpenAsync();
            await using var command = await CreateCommandAsync("GetChatHistory", CommandType.StoredProcedure,
                new SqlParameter("userId",userId)
            );
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ToList<ChatListItemWithLastContentDto>();
        }

        public async Task<List<ChatContentDto>> GetChatContentAsync(ChatContentRequestDto model)
        {
            await EnsureConnectionOpenAsync();
            await using var command = await CreateCommandAsync("GetChatContent", CommandType.StoredProcedure,
                new SqlParameter("count",model.Count),
                new SqlParameter("baseDateTime",model.BaseDateTime),
                new SqlParameter("chatId",model.ChatId)
            );
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ToList<ChatContentDto>();
        }

        public async Task<ChatListItemDto> StartPrivateChatAsync(long userId, long userReceiverId)
        {

            var receiver = await _userRepository.FirstOrDefaultAsync(x => x.Id == userReceiverId);
            if (receiver == null) throw new EntityNotFoundException(typeof(User), userReceiverId);

            var chatId = await GetChatAsync(userId, userReceiverId);
            if (chatId.HasValue)
            {
                return new ChatListItemDto()
                {
                    ChatId = chatId.Value,
                    LogoAddress = receiver.AvatarAddress,
                    Title = receiver.FullName,
                    UnreadCount = 0
                };
            }

            var sender = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            if (sender == null) throw new EntityNotFoundException(typeof(User), userId);

            var chat = new Chat
            {
                Id = SequentialGuidGenerator.Instance.Create(),
                ChatType = ChatType.Simple,
            };
            chat.Participants.Add(new ChatParticipant
            {
                UserId = sender.Id,
                Title = receiver.FullName,
                ChatAccessType = ChatAccessType.Admin,
                ChatId = chat.Id,
                LogoAddress = receiver.AvatarAddress,
            });
            chat.Participants.Add(new ChatParticipant
            {
                UserId = receiver.Id,
                Title = sender.FullName,
                ChatAccessType = ChatAccessType.Admin,
                ChatId = chat.Id,
                LogoAddress = sender.AvatarAddress,
            });
            _ = await InsertAsync(chat);
            return new ChatListItemDto()
            {
                ChatId = chat.Id,
                LogoAddress = receiver.AvatarAddress,
                Title = receiver.FullName,
                UnreadCount = 0
            };


        }

        public async Task InsertContentAsync(ChatContentDto model)
        {
            await EnsureConnectionOpenAsync();
            await using var command = await CreateCommandAsync("InsertChatContent", CommandType.StoredProcedure,
                new SqlParameter("id",model.Id),
                new SqlParameter("userId",model.UserId),
                new SqlParameter("chatId",model.ChatId),
                new SqlParameter("replyOfId",(object)model.ReplyOfId ?? DBNull.Value),
                new SqlParameter("text",(object)model.Text?? DBNull.Value),
                new SqlParameter("mediaAddress",(object)model.MediaAddress?? DBNull.Value),
                new SqlParameter("thumbnailAddress",(object)model.ThumbnailAddress?? DBNull.Value)
            );
            _ = await command.ExecuteNonQueryAsync();
        }

        public async Task ClearAllAsync()
        {
            await EnsureConnectionOpenAsync();
            var query = @$"delete from ChatParticipants
                           delete from ChatContents
                           delete from Chats";
            await using var command = await CreateTSqlCommandAsync(query);
            _ = await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> CanGetContentAsync(long userId, Guid chatId)
        {
            return await (await GetContextAsync()).ChatParticipants.AnyAsync(x => x.ChatId == chatId && x.UserId == userId);
        }

        private async Task<Guid?> GetChatAsync(long userId, long receiverId)
        {
            await EnsureConnectionOpenAsync();
            await using var command = await CreateCommandAsync("GetChat", CommandType.StoredProcedure,
                new SqlParameter("userId",userId),
                new SqlParameter("receiverId",receiverId)
            );
            await using var reader = await command.ExecuteReaderAsync();
            return (await reader.ToList<GetChatDto>()).FirstOrDefault()?.ChatId;
        }
    }
}

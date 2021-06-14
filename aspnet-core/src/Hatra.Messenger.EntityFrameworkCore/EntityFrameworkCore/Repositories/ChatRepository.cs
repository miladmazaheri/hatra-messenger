﻿using System;
using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    public interface IChatRepository : IRepository<Chat, Guid>
    {
        Task<List<ChatListItemWithLastContentDto>> GetChatHistoryAsync(long userId);
        Task<ChatListItemDto> StartPrivateChatAsync(long userId, long userReceiverId);
        Task InsertContentAsync(ChatContentDto model);
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
            var query = @$"EXEC [dbo].[GetChatHistory] @userId = {userId}";
            await using var command = await CreateTSqlCommandAsync(query);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ToList<ChatListItemWithLastContentDto>();
        }

        public async Task<ChatListItemDto> StartPrivateChatAsync(long userId, long userReceiverId)
        {
            //TODO چک بشه که چت تکراری نباشه
            var sender = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            if (sender == null) throw new EntityNotFoundException(typeof(User), userId);
            var receiver = await _userRepository.FirstOrDefaultAsync(x => x.Id == userReceiverId);
            if (receiver == null) throw new EntityNotFoundException(typeof(User), userReceiverId);

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
            var query = @$"EXEC [dbo].[InsertChatContent]
                            @id = {model.Id},
                            @userId = {model.UserId},
                            @chatId = {model.Id},
                            @replyOfId = {(model.ReplyOfId.HasValue ? $"'{model.ReplyOfId}'" : "NULL")},
                            @text = {(!string.IsNullOrWhiteSpace(model.Text) ? $"N'{model.Text}'" : "NULL")},
                            @mediaAddress = {(!string.IsNullOrWhiteSpace(model.MediaAddress) ? $"N'{model.MediaAddress}'" : "NULL")},
                            @thumbnailAddress = {(!string.IsNullOrWhiteSpace(model.ThumbnailAddress) ? $"N'{model.ThumbnailAddress}'" : "NULL")}";
            await using var command = await CreateTSqlCommandAsync(query);
            _ = await command.ExecuteNonQueryAsync();
        }

    }
}

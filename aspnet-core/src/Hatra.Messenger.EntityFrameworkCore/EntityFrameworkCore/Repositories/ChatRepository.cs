using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Hatra.Messenger.Chats.Entities;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Tools;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    public interface IChatRepository : IRepository<Chat, Guid>
    {
        Task<List<ChatListItemDto>> GetChatHistoryAsync(long userId);
    }
    public class ChatRepository : MessengerRepositoryBase<Chat, Guid>, IChatRepository
    {
        public ChatRepository(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider) : base(dbContextProvider, transactionProvider)
        {
        }

        public async Task<List<ChatListItemDto>> GetChatHistoryAsync(long userId)
        {
            await EnsureConnectionOpenAsync();
            var query = @$"EXEC [dbo].[GetChatHistory] @userId = {userId}";
            await using var command = await CreateTSqlCommandAsync(query);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ToList<ChatListItemDto>();
        }
    }
}

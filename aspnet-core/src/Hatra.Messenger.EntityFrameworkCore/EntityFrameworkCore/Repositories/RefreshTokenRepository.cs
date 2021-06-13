using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.EntityFrameworkCore;
using Hatra.Messenger.EntityFrameworkCore.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken, long>
    {
        Task InsertOrUpdateAsync(long userId, string token, string ip, string device, DateTime expires);
    }
    public class RefreshTokenRepository : MessengerRepositoryBase<RefreshToken, long>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider) : base(dbContextProvider,transactionProvider)
        {
        }

        public async Task InsertOrUpdateAsync(long userId, string token, string ip, string device, DateTime expires)
        {
            await EnsureConnectionOpenAsync();

            var query = @$"EXEC [dbo].[InsertOrUpdateRefreshToken] @userId={userId},@token=N'{token}',@device=N'{device}',@ip=N'{ip}',@expires='{expires}'";
            await using var command = await CreateTSqlCommandAsync(query);

            _ = await command.ExecuteNonQueryAsync();
        }

        
    }
}

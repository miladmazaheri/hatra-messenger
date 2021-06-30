using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Common.Users;
using Hatra.Messenger.Tools;
using Microsoft.Data.SqlClient;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    public interface IUserRepository : IRepository<User, long>
    {
        Task<List<UserInfoDto>> GetAllByPhoneListAsync(List<string> phones);
    }
    public class UserRepository : MessengerRepositoryBase<User, long>, IUserRepository
    {
        public UserRepository(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider) : base(dbContextProvider, transactionProvider)
        {
        }

        public async Task<List<UserInfoDto>> GetAllByPhoneListAsync(List<string> phones)
        {
            if (phones == null || phones.Count == 0) return new List<UserInfoDto>();

            var data = phones.Select(x => $"(N'{x}')").Aggregate((a, b) => a + ",\n" + b);

            var query = @$"declare @Temp table (Phone nvarchar(11))
insert into @Temp(Phone)values
{data}
select u.Id,u.UserName as Username,u.Name+' '+u.Surname as FullName,u.AvatarAddress,u.Status from AbpUsers u join @Temp t on u.PhoneNumber = t.Phone";

            await EnsureConnectionOpenAsync();
            await using var command = await CreateCommandAsync(query, CommandType.Text);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ToList<UserInfoDto>();
        }
    }
}

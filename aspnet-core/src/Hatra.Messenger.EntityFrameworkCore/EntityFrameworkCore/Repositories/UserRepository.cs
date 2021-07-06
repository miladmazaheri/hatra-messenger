using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Common.DataTransferObjects;
using Hatra.Messenger.Common.Users;
using Hatra.Messenger.Tools;
using Microsoft.Data.SqlClient;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    public interface IUserRepository : IRepository<User, long>
    {
        Task<List<UserInfoDto>> GetContactsAsync(List<string> phones, List<string> usernames);
    }
    public class UserRepository : MessengerRepositoryBase<User, long>, IUserRepository
    {
        public UserRepository(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider) : base(dbContextProvider, transactionProvider)
        {
        }

        public async Task<List<UserInfoDto>> GetContactsAsync(List<string> phones, List<string> usernames)
        {
            if ((phones == null || phones.Count == 0) && (usernames == null || usernames.Count == 0)) return new List<UserInfoDto>();

            var phoneAgg = phones?.Any() ?? false ? phones.Select(x => $"(N'{x}')").Aggregate((a, b) => a + ",\n" + b) : null;
            var usernameAgg = usernames?.Any() ?? false ? usernames?.Select(x => $"(N'{x}')").Aggregate((a, b) => a + ",\n" + b) : null;

            string query;
            if (usernameAgg.IsNullOrWhiteSpace())
            {
                query = @$"declare @Temp table (Phone nvarchar(11))
insert into @Temp(Phone)values
{phoneAgg}
select u.Id,u.UserName as Username,u.Name+' '+u.Surname as FullName,u.AvatarAddress,u.Status from AbpUsers u join @Temp t on u.PhoneNumber = t.Phone";
            }

            else if (phoneAgg.IsNullOrWhiteSpace())
            {
                query = $@"declare @UsernameTemp table (Username nvarchar(256))
insert into @UsernameTemp(Username)values
{usernameAgg}
  select 
u.Id,
u.UserName as Username,
u.Name+' '+u.Surname as FullName,
u.AvatarAddress,
u.Status,
null as PhoneNumber
 from AbpUsers u 
 join @UsernameTemp t on u.UserName = t.Username";
            }

            else
            {
                query = @$"declare @PhoneTemp table (Phone nvarchar(11))
declare @UsernameTemp table (Username nvarchar(256))
insert into @PhoneTemp(Phone)values
{phoneAgg}
insert into @UsernameTemp(Username)values
{usernameAgg}

select 
u.Id,
u.UserName as Username,
u.Name+' '+u.Surname as FullName,
u.AvatarAddress,
u.Status,
u.PhoneNumber
 from AbpUsers u 
 join @PhoneTemp t on u.PhoneNumber = t.Phone
  UNION 
  select 
u.Id,
u.UserName as Username,
u.Name+' '+u.Surname as FullName,
u.AvatarAddress,
u.Status,
null as PhoneNumber
 from AbpUsers u 
 join @UsernameTemp t on u.UserName = t.Username";
            }



            await EnsureConnectionOpenAsync();
            await using var command = await CreateCommandAsync(query, CommandType.Text);
            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ToList<UserInfoDto>();
        }
    }
}

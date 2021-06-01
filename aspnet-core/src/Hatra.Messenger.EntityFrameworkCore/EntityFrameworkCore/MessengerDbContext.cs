using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Hatra.Messenger.Authorization.Roles;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.MultiTenancy;

namespace Hatra.Messenger.EntityFrameworkCore
{
    public class MessengerDbContext : AbpZeroDbContext<Tenant, Role, User, MessengerDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
            : base(options)
        {
        }
    }
}

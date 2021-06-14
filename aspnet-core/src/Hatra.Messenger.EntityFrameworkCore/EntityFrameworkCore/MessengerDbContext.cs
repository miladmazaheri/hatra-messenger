using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Hatra.Messenger.Authorization.Roles;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Chats;
using Hatra.Messenger.Chats.Entities;
using Hatra.Messenger.MultiTenancy;

namespace Hatra.Messenger.EntityFrameworkCore
{
    public class MessengerDbContext : AbpZeroDbContext<Tenant, Role, User, MessengerDbContext>
    {
        public virtual DbSet<Chat> Chats{ get; set; }
        public virtual DbSet<ChatContent> ChatContents{ get; set; }
        public virtual DbSet<ChatParticipant> ChatParticipants{ get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens{ get; set; }

        public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureChat();
        }
    }
}

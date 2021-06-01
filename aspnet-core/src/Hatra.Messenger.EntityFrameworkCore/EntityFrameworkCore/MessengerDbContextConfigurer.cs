using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Hatra.Messenger.EntityFrameworkCore
{
    public static class MessengerDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<MessengerDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<MessengerDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}

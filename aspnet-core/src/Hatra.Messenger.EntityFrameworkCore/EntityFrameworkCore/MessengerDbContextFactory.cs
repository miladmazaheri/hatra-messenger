using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Hatra.Messenger.Configuration;
using Hatra.Messenger.Web;

namespace Hatra.Messenger.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class MessengerDbContextFactory : IDesignTimeDbContextFactory<MessengerDbContext>
    {
        public MessengerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MessengerDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            MessengerDbContextConfigurer.Configure(builder, configuration.GetConnectionString(MessengerConsts.ConnectionStringName));

            return new MessengerDbContext(builder.Options);
        }
    }
}

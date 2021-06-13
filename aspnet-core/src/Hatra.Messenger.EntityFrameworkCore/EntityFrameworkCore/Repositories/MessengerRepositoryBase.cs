using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Abp.Data;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.Data.SqlClient;

namespace Hatra.Messenger.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class MessengerRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<MessengerDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IActiveTransactionProvider _transactionProvider;

        protected MessengerRepositoryBase(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
        }

        // Add your common methods for all repositories
        protected async Task<DbCommand> CreateTSqlCommandAsync(string commandText)
        {
            var command = (await GetConnectionAsync()).CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            command.Transaction = GetActiveTransaction();
            return command;
        }

        protected async Task<DbCommand> CreateCommandAsync(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var command = (await GetConnectionAsync()).CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = GetActiveTransaction();

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        protected async Task EnsureConnectionOpenAsync()
        {
            var connection = await GetConnectionAsync();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
        }

        protected DbTransaction GetActiveTransaction()
        {
            return (DbTransaction)_transactionProvider.GetActiveTransaction(new ActiveTransactionProviderArgs
            {
                {"ContextType", typeof(MessengerDbContext) },
                {"MultiTenancySide", MultiTenancySide }
            });
        }
    }

    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="MessengerRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class MessengerRepositoryBase<TEntity> : MessengerRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {

        // Do not add any method here, add to the class above (since this inherits it)!!!
        protected MessengerRepositoryBase(IDbContextProvider<MessengerDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider) : base(dbContextProvider, transactionProvider)
        {
        }
    }


}

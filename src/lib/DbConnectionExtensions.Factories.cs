using System.Data;
using System.Data.Common;

namespace Ockham.Data
{

    public static partial class DbConnectionExtensions
    {

        /// <summary>
        /// Create a <see cref="DbCommand"/> from the provided <paramref name="connection"/> and initialize it
        /// with the provided <paramref name="sql"/>, <paramref name="parameters"/>, and <paramref name="commandType"/>
        /// </summary>
        /// <param name="connection">An open <see cref="DbConnection"/></param>
        /// <param name="sql">The complete SQL statement to execute in provider-specific SQL syntax</param>
        /// <param name="parameters">See <see cref="DbParameterExtensions.AddParameters(DbParameterCollection, object)"/></param>
        /// <param name="commandType">The <see cref="CommandType"/> for the command</param>
        public static DbCommand CreateCommand(this DbConnection connection, string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            if (parameters != null)
            {
                command.AddParameters(parameters);
            }
            return command;
        }

        /// <summary>
        /// Get the <see cref="DbProviderFactory"/> related to this <see cref="DbConnection"/>,
        /// optionally throwing an exception if no factory is found.
        /// See <see cref="Ockham.Data.Common.DbProviderFactories.GetFactory(DbConnection, bool)"/>
        /// </summary>
        public static DbProviderFactory GetFactory(this DbConnection connection, bool errorIfNull)
        {
            return Ockham.Data.Common.DbProviderFactories.GetFactory(connection, errorIfNull);
        }

        /// <summary>
        /// Create a new <see cref="DbDataAdapter"/> for the same provided as this <see cref="DbConnection"/>.
        /// </summary>
        public static DbDataAdapter CreateDataAdapter(this DbConnection connection)
        {
            return connection.GetFactory(true).CreateDataAdapter();
        }

    }

}
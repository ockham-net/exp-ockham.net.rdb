using System.Data.Common;

namespace Ockham.Data
{
    /// <summary>
    /// Extensions to <see cref="DbConnection"/>
    /// </summary>
    public static partial class DbConnectionExtensions
    {
        /// <summary>
        /// Execute a SQL statement that does not return any rows. See <see cref="DbCommand.ExecuteNonQuery"/>
        /// </summary>
        /// <param name="connection">An open <see cref="DbConnection"/></param>
        /// <param name="sql">The complete SQL statement to execute in provider-specific SQL syntax</param>
        /// <param name="parameters">See <see cref="DbParameterExtensions.AddParameters(DbParameterCollection, object)"/></param>
        /// <returns>Provider-defined. Usually the number of rows affected</returns>
        public static int Execute(this DbConnection connection, string sql, object parameters = null)
        {
            using (var lCommand = connection.CreateCommand(sql, parameters))
            {
                return lCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute a SQL statement that returns a single scalar value. See <see cref="DbCommand.ExecuteScalar"/>
        /// </summary>
        /// <param name="connection">An open <see cref="DbConnection"/></param>
        /// <param name="sql">The complete SQL statement to execute in provider-specific SQL syntax</param>
        /// <param name="parameters">See <see cref="DbParameterExtensions.AddParameters(DbParameterCollection, object)"/></param>
        /// <returns>The scalar value as returned from the underlying provider connection</returns>
        public static object ExecuteScalar(this DbConnection connection, string sql, object parameters = null)
        {
            using (var lCommand = connection.CreateCommand(sql, parameters))
            {
                return lCommand.ExecuteScalar();
            }
        }
    }
}

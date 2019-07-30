using System;
using System.Data;
using System.Data.Common;
using Ockham.Data.Extensions;
using static Ockham.Data.Extensions.Delegates;

namespace Ockham.Data
{
    public static partial class DbConnectionExtensions
    {
        /// <summary>
        /// Execute a named stored procedure
        /// </summary>
        /// <param name="connection">An open <see cref="DbConnection"/></param>
        /// <param name="procedureName">The name of the stored procedure to invoke</param>
        /// <param name="parameters">See <see cref="DbParameterExtensions.AddParameters(DbParameterCollection, object)"/></param>
        /// <returns>Provider-defined. Usually the number of rows affected</returns>
        public static int ExecuteProcedure(this DbConnection connection, string procedureName, object parameters = null)
        {
            if (ProviderExtensions.ExecuteProcedure.TryGetDelegate(connection, out ExecuteProcedure @delegate))
            {
                return @delegate(connection, procedureName, parameters);
            }
            return ExecuteProcedureGeneric(connection, procedureName, parameters);
        }

        private static int ExecuteProcedureGeneric(this DbConnection connection, string procedureName, object parameters)
        {
            using (var lCommand = connection.CreateCommand(procedureName, parameters, CommandType.StoredProcedure))
            {
                return lCommand.ExecuteNonQuery();
            }
        }
    }
}

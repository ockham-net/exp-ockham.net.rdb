using Ockham.Data.Extensions;
using System.Data.SqlClient;

namespace Ockham.Data.SqlClient
{
    public static class SqlExtensions
    {
        public const string ProviderInvariantName = "System.Data.SqlClient";

        private static bool _loaded = false;

        private static readonly object _lock = new object();

        public static void LoadExtensions()
        {
            if (_loaded) return;
            lock (_lock)
            {
                if (_loaded) return;

                ProviderExtensions.CopyParameter.Add(
                    ProviderInvariantName,
                    (source, target) => SqlParameterExtensions.CopyTo(source as SqlParameter, target as SqlParameter)
                );

                ProviderExtensions.AddNullableParameter.Add(
                    ProviderInvariantName,
                    (parameters, name, value) => SqlParameterExtensions.AddNullable(parameters as SqlParameterCollection, name, value)
                );

                ProviderExtensions.ExecuteProcedure.Add(
                    ProviderInvariantName,
                    (connection, procedureName, parameters) => SqlConnectionExtensions.ExecuteProcedure(connection as SqlConnection, procedureName, parameters)
                );
                 
                ProviderExtensions.FillTable.Add(ProviderInvariantName, SqlConnectionExtensions.FillTable);

                _loaded = true;
            }
        }
    }
}

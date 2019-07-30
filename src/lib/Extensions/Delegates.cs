using Ockham.RdbInternal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using static Ockham.Data.Common.DbProviderFactories;

namespace Ockham.Data.Extensions
{
    public static class Delegates
    {
        /// <summary>
        /// Executes a stored procedure on the target connection.
        /// See <see cref="DbConnectionExtensions.ExecuteProcedure(DbConnection, string, object)" />
        /// </summary> 
        public delegate int ExecuteProcedure(DbConnection connection, string procedureName, object parameters);

        /// <summary>
        /// Uploads the provided source table to the target database. 
        /// See <see cref="DbConnectionExtensions.FillTable(DbConnection, DataTable, string, bool, IDictionary{string, string})"/>
        /// </summary> 
        public delegate int FillTable(DbConnection connection, DataTable source, string targetName, bool byName, IDictionary<string, string> nameMap);

        /// <summary>
        /// Copy all properties from the source to the target parameter object.
        /// </summary> 
        public delegate void CopyParameter(DbParameter source, DbParameter target);

        /// <summary>
        /// Add a parameter to the target parameter collection, automatically converting any null parameter value to <see cref="DBNull"/>
        /// </summary>
        public delegate DbParameter AddNullableParameter(DbParameterCollection parameters, string parameterName, object parameterValue);

        /// <summary>
        /// Ensure the provided identifier is quoted per the target database syntax rules
        /// </summary>
        public delegate string QuoteIdentifier(string identifier);

        /// <summary>
        /// Get the deleate type for the speicified extension method uri
        /// </summary>
        public static Type GetDelegate(string methodUri)
        {
            return _nameMap.ItemOrDefault(NormalizeNamespace(methodUri) ?? throw new ArgumentNullException("methodUri"));
        }

        private static readonly Dictionary<string, Type> _nameMap
            = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { ProviderExtensions.ExtensionUris.Methods.AddNullableParameter, typeof(AddNullableParameter) },
                { ProviderExtensions.ExtensionUris.Methods.CopyParameter, typeof(CopyParameter) },
                { ProviderExtensions.ExtensionUris.Methods.ExecuteProcedure, typeof(ExecuteProcedure) },
                { ProviderExtensions.ExtensionUris.Methods.FillTable, typeof(FillTable) },
                { ProviderExtensions.ExtensionUris.Methods.QuoteIdentifier, typeof(QuoteIdentifier) }
            };

    }

}

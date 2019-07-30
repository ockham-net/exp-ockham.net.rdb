using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Ockham.Data.Extensions
{
    /// <summary>
    /// Container for delegates for methods that can be extended with provider-specific implementations.
    /// See for example <see cref="ProviderExtensions.FillTable"/>, <see cref="ExtensionSet{TDelegate}.Add(string, TDelegate)"/>
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        /// Copy all properties from the source to the target parameter object.
        /// See <see cref="DbParameterExtensions.CopyTo(DbParameter, DbParameter)"/>
        /// </summary> 
        public delegate void CopyParameter(DbParameter source, DbParameter target);

        /// <summary>
        /// Add a parameter to the target parameter collection, automatically converting any null parameter value to <see cref="DBNull"/>.
        /// See <see cref="DbParameterExtensions.AddNullable(DbParameterCollection, string, object)"/>
        /// </summary>
        public delegate DbParameter AddNullableParameter(DbParameterCollection parameters, string parameterName, object parameterValue);

        /// <summary>
        /// Executes a stored procedure on the target connection. 
        /// See <see cref="DbConnectionExtensions.ExecuteProcedure(DbConnection, string, object)"/>
        /// </summary> 
        public delegate int ExecuteProcedure(DbConnection connection, string procedureName, object parameters);

        /// <summary>
        /// Uploads the provided source table to the target database.
        /// See <see cref="DbConnectionExtensions.FillTable(DbConnection, DataTable, string, bool, IDictionary{string, string})"/>
        /// </summary> 
        public delegate int FillTable(DbConnection connection, DataTable source, string targetName, bool byName, IDictionary<string, string> nameMap);

        /// <summary>
        /// Ensure the provided identifier is quoted per the target database syntax rules.
        /// See <see cref="DbConnectionExtensions.QuoteIdentifier(DbConnection, string)"/>
        /// </summary>
        public delegate string QuoteIdentifier(DbConnection connection, string identifier);
    }
}

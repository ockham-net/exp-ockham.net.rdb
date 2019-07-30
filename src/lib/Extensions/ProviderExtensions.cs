using System.Data.Common;
using System.Collections.Generic;
using System.Data;

using static Ockham.Data.Extensions.Delegates;

namespace Ockham.Data.Extensions
{
    /// <summary>
    /// Loads and indexes available exension methods by provider invariant name (namespace), such as "System.Data.SqlClient" or "MySql.Data.MySqlClient"
    /// </summary>
    public static partial class ProviderExtensions
    {
        /// <summary>
        /// Provider-specific implemenations of <see cref="DbParameterExtensions.CopyTo(DbParameter, DbParameter)"/>
        /// </summary>
        public static ExtensionSet<CopyParameter> CopyParameter { get; } = new ExtensionSet<CopyParameter>();

        /// <summary>
        /// Provider-specific implemenations of <see cref="DbConnectionExtensions.FillTable(DbConnection, DataTable, string, bool, IDictionary{string, string})"/>
        /// </summary>
        public static ExtensionSet<FillTable> FillTable { get; } = new ExtensionSet<FillTable>();

        /// <summary>
        /// Provider-specific implemenations of <see cref="DbConnectionExtensions.ExecuteProcedure(DbConnection, string, object)"/>
        /// </summary>
        public static ExtensionSet<ExecuteProcedure> ExecuteProcedure { get; } = new ExtensionSet<ExecuteProcedure>();

        /// <summary>
        /// Provider-specific implemenations of <see cref="DbParameterExtensions.AddNullable(DbParameterCollection, string, object)"/>
        /// </summary>
        public static ExtensionSet<AddNullableParameter> AddNullableParameter { get; } = new ExtensionSet<AddNullableParameter>();

        /// <summary>
        /// Provider-specific implemenations of <see cref="DbConnectionExtensions.QuoteIdentifier(DbConnection, string)"/>
        /// </summary>
        public static ExtensionSet<QuoteIdentifier> QuoteIdentifier { get; } = new ExtensionSet<QuoteIdentifier>();

    }
}

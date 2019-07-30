using Ockham.Data.Extensions;
using Ockham.Reflection;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Ockham.Data.SqlClient
{
    [ExtensionClass(ProviderExtensions.ExtensionUris.Scheme)]
    public static class SqlParameterExtensions
    {

        [ExtensionMethod(ProviderExtensions.ExtensionUris.Scheme, typeof(Delegates.FillTable), "System.Data.SqlClient")]
        public static void CopyTo(this SqlParameter source, SqlParameter target)
        {
            DbParameterExtensions.CopyCommonProps(source, target);
            CopySqlProps(source, target);
        }

        private static void CopySqlProps(SqlParameter source, SqlParameter target)
        {
            target.CompareInfo = source.CompareInfo;
            target.IsNullable = source.IsNullable;
            target.LocaleId = source.LocaleId;
            target.Offset = source.Offset;
            target.SqlDbType = source.SqlDbType;
            target.TypeName = source.TypeName;
            target.UdtTypeName = source.UdtTypeName;
            target.XmlSchemaCollectionDatabase = source.XmlSchemaCollectionDatabase;
            target.XmlSchemaCollectionName = source.XmlSchemaCollectionName;
            target.XmlSchemaCollectionOwningSchema = source.XmlSchemaCollectionOwningSchema;
            target.SqlValue = source.SqlValue ?? DBNull.Value;
        }

        [ExtensionMethod(ProviderExtensions.ExtensionUris.Scheme, "System.Data.SqlClient")]
        internal static DbParameter AddNullable(DbParameterCollection parameters, string parameterName, object parameterValue)
            => AddNullable(parameters as SqlParameterCollection, parameterName, parameterValue);

        public static SqlParameter AddNullable(this SqlParameterCollection parameters, string parameterName, object parameterValue)
        {
            return parameters.AddWithValue(parameterName, parameterValue ?? DBNull.Value);
        }

    }
}

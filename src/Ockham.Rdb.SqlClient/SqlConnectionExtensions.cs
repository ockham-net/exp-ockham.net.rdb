using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace Ockham.Data.SqlClient
{
    public static class SqlConnectionExtensions
    {
        internal static int FillTable(DbConnection connection, DataTable source, string targetName, bool byName, IDictionary<string, string> nameMap)
            => FillTable(connection as SqlConnection, source, targetName, byName, nameMap);

        public static int FillTable(this SqlConnection connection, DataTable source, string targetName, bool byName, IDictionary<string, string> nameMap)
        {
            int lRowsAffected;

            using (var lBulkCopy = new SqlBulkCopy(connection))
            {
                lBulkCopy.BulkCopyTimeout = 0;

                lBulkCopy.DestinationTableName = string.IsNullOrEmpty(targetName) ? source.TableName : targetName;

                if (nameMap != null)
                {
                    foreach (string lTargetColumn in nameMap.Keys)
                    {
                        lBulkCopy.ColumnMappings.Add(nameMap[lTargetColumn], lTargetColumn);
                    }
                }
                else if (byName)
                {
                    foreach (DataColumn lCol in source.Columns)
                    {
                        lBulkCopy.ColumnMappings.Add(lCol.ColumnName, lCol.ColumnName);
                    }
                }

                lBulkCopy.WriteToServer(source);
                lRowsAffected = source.Rows.Count;
            }

            return lRowsAffected;
        }

        internal static int ExecuteProcedure(DbConnection connection, string procedureName, object parameters = null)
            => ExecuteProcedure(connection as SqlConnection, procedureName, parameters);

        public static int ExecuteProcedure(this SqlConnection connection, string procedureName, object parameters = null)
        {
            using (var lCommand = (SqlCommand)connection.CreateCommand(procedureName, parameters, CommandType.StoredProcedure))
            {

                var lReturnParam = lCommand.Parameters.Add("P_" + Guid.NewGuid().ToString("n"), SqlDbType.Int);
                lReturnParam.Direction = ParameterDirection.ReturnValue;

                lCommand.ExecuteNonQuery();

                return System.Convert.ToInt32(lReturnParam.Value);
            }
        }

    }
}

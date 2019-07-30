using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Ockham.Data.Extensions;
using static Ockham.Data.Extensions.Delegates;

namespace Ockham.Data
{
    public static partial class DbConnectionExtensions
    {
        public static int FillTable(this DbConnection connection, DataTable source, string targetName = null, bool byName = false, IDictionary<string, string> nameMap = null)
        {
            if (ProviderExtensions.TryGetExtension(connection, out FillTable fillTable))
            {
                return fillTable(connection, source, targetName, byName, nameMap);
            }

            return GenericFillTable(connection, source, targetName, byName, nameMap);
        }

        private static int GenericFillTable(this DbConnection connection, DataTable source, string targetName, bool byName, IDictionary<string, string> nameMap)
        {
            string lTargetTable = string.IsNullOrEmpty(targetName) ? source.TableName : targetName;
            DataTable lRemoteTable = new DataTable();
            lRemoteTable.Locale = source.Locale;
            int lRowsAffected;

            var lFactory = connection.GetFactory(true);

            using (var lAdapter = lFactory.CreateDataAdapter())
            using (var lBuilder = lFactory.CreateCommandBuilder())
            {
                lBuilder.DataAdapter = lAdapter;
                Func<string, string> fnQuote = null;

                var lFieldNames = new List<string>();
                if (nameMap != null)
                {
                    fnQuote = connection.GetQuoteIdentifierDelegate();
                    foreach (string lFieldName in nameMap.Values)
                    {
                        lFieldNames.Add(fnQuote(lFieldName));
                    }
                }

                string lSourceColumns = "*";
                if (nameMap != null)
                {
                    lSourceColumns = string.Join(", ", lFieldNames);
                }
                else if (byName)
                {
                    fnQuote = fnQuote ?? connection.GetQuoteIdentifierDelegate();
                    DataTable lAllColsTable = connection.OpenTable("SELECT * FROM " + targetName + " WHERE 1=0");
                    var lColNames = new List<string>();
                    nameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataColumn lCol in source.Columns)
                    {
                        string lColName = lCol.ColumnName;
                        if (lAllColsTable.Columns.Contains(lColName))
                        {
                            lColNames.Add(fnQuote(lColName));
                            nameMap.Add(lColName, lColName);
                        }
                    }
                    lSourceColumns = string.Join(", ", lColNames);
                }

                string lSelectSQL = string.Format("SELECT {0} FROM {1} WHERE 1=0", lSourceColumns, targetName);
                lAdapter.SelectCommand = connection.CreateCommand(lSelectSQL);
                lAdapter.Fill(lRemoteTable);

                if (nameMap != null)
                {
                    DataTableCopier.CopyTo(source, lRemoteTable, new Dictionary<string, string>(nameMap));
                }
                else
                {
                    DataTableCopier.CopyTo(source, lRemoteTable, byName);
                }

                // Use the command builder to automatically generate the insert command
                lAdapter.InsertCommand = lBuilder.GetInsertCommand();

                lRowsAffected = lAdapter.Update(lRemoteTable);
            }


            return lRowsAffected;
        }

    }
}

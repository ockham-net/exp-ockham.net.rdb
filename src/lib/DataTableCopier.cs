using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Ockham.Data
{
    public static class DataTableCopier
    {
        /// <summary>
        /// Copy all rows from <paramref name="source"/> to <paramref name="target"/> using
        /// <see cref="DataRow.ItemArray"/> and <see cref="DataRowCollection.Add(object[])"/>   
        /// </summary>
        public static void CopyTo(DataTable source, DataTable target)
        {
            int lng = source.Rows.Count;
            for (int i = 0; i < lng; i++)
            {
                target.Rows.Add(source.Rows[i].ItemArray);
            }
        }

        /// <summary>
        /// Copy all rows from <paramref name="source"/> to <paramref name="target"/>. If <paramref name="byName"/>
        /// is false, rows are columns are copied in positional order using <see cref="CopyTo(DataTable, DataTable)"/>. 
        /// If <paramref name="byName"/> is true, only columns with names matched in both tables are copied. 
        /// </summary>
        public static void CopyTo(DataTable source, DataTable target, bool byName, StringComparer comparer = null)
        {
            if (!byName) { CopyTo(source, target); return; }

            comparer = comparer ?? StringComparer.OrdinalIgnoreCase;
            var table1Cols = new Dictionary<string, int>(comparer);
            var table2Cols = new Dictionary<string, int>(comparer);
        }

        public static void CopyTo(DataTable source, DataTable target, Dictionary<string, string> nameMap)
        {

        }

        public static DataTable FromDataReader(DbDataReader reader, bool firstRowOnly)
        {
            throw null;
        }
    }
}

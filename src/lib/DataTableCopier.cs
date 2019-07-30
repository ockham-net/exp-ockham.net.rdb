using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Ockham.Data
{
    /// <summary>
    /// Utility for copying data from one <see cref="DataTable"/> to another
    /// </summary>
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

            throw new NotImplementedException();
        }

        /// <summary>
        /// Copy all rows from <paramref name="source"/> to <paramref name="target"/>, using
        /// the provided map of target column name to source column name
        /// </summary>
        public static void CopyTo(DataTable source, DataTable target, Dictionary<string, string> nameMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new <see cref="DataTable"/> from an open <see cref="DbDataReader"/>
        /// </summary>
        /// <param name="reader">The open <see cref="DbDataReader"/> to read from</param>
        /// <param name="firstRowOnly">Whether to read only the first row</param>
        public static DataTable FromDataReader(DbDataReader reader, bool firstRowOnly)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Data;
using System.Data.Common;

namespace Ockham.Data
{

    public static partial class DbConnectionExtensions
    {

        public static DataTable OpenTable(this DbConnection connection, string sql, object parameters = null)
        {
            using (var lCommand = connection.CreateCommand(sql, parameters))
            {
                using (var lReader = lCommand.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    var lTable = DataTableCopier.FromDataReader(lReader, true);
                    return lTable;
                }
            }
        }

        public static DataRow OpenFirstRow(this DbConnection connection, string sql, object parameters = null)
        {
            using (var lCommand = connection.CreateCommand(sql, parameters))
            {
                using (var lReader = lCommand.ExecuteReader(CommandBehavior.SingleRow))
                {
                    var lTable = DataTableCopier.FromDataReader(lReader, true);
                    if (lTable.Rows.Count == 0) return null;
                    return lTable.Rows[0];
                }
            }
        } 

    }
}

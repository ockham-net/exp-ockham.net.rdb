using System.Data.Common;

namespace Ockham.Data
{

    public static partial class DbConnectionExtensions
    {
        public static int Execute(this DbConnection connection, string sql, object parameters = null)
        {
            using (var lCommand = connection.CreateCommand(sql, parameters))
            {
                return lCommand.ExecuteNonQuery();
            }
        }
         
        public static object ExecuteScalar(this DbConnection connection, string sql, object parameters = null)
        {
            using (var lCommand = connection.CreateCommand(sql, parameters))
            {
                return lCommand.ExecuteScalar();
            }
        }

    }
}

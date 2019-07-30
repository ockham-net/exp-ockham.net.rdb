using System.Data;
using System.Data.Common;

namespace Ockham.Data
{

    public static partial class DbConnectionExtensions
    {

        public static DbCommand CreateCommand(this DbConnection connection, string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            if (parameters != null)
            {
                command.AddParameters(parameters);
            }
            return command;
        }

        //public static DbProviderFactory GetFactory(this DbConnection connection)
        //{
        //    return Ockham.Data.Common.DbProviderFactories.GetFactory(connection);
        //}

        public static DbProviderFactory GetFactory(this DbConnection connection, bool errorIfNull)
        {
            return Ockham.Data.Common.DbProviderFactories.GetFactory(connection, errorIfNull);
        }

        //public static DbParameterCollection CreateParameters(this DbConnection connection, object source = null)
        //{
        //    var command = connection.CreateCommand();
        //    if (source != null) command.AddParameters(source);
        //    return command.Parameters;
        //}

        public static DbDataAdapter CreateDataAdapter(this DbConnection connection)
        {
            return connection.GetFactory(true).CreateDataAdapter();
        }

        //public static DbParameter CreateParameter(this DbConnection connection)
        //{
        //    return connection.GetFactory(true).CreateParameter();
        //}


    }

}
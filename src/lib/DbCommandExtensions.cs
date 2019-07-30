using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Ockham.Data
{
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Add data from <paramref name="source"/> to the <see cref="DbCommand.Parameters"/> collection of the command.
        /// See <see cref="DbParameterExtensions.AddParameters(DbParameterCollection, object)"/>
        /// </summary>
        public static void AddParameters(this DbCommand command, object source)
        {
            DbParameterExtensions.AddParameters(command.Parameters, source);
        }
    }
}

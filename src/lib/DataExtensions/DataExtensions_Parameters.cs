using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using Ockham.Data.Extensions;
using static Ockham.Data.Extensions.Delegates;

namespace Ockham.Data
{
    public static partial class DataExtensions
    {
       

        
        //public static OdbcParameter AddNullable(this OdbcParameterCollection parameters, string parameterName, object parameterValue)
        //{
        //    return parameters.AddWithValue(parameterName, parameterValue ?? DBNull.Value);
        //}

        //public static OleDbParameter AddNullable(this OleDbParameterCollection parameters, string parameterName, object parameterValue)
        //{
        //    return parameters.AddWithValue(parameterName, parameterValue ?? DBNull.Value);
        //}


      
       
        //private static void CopyOleDbProps(OleDbParameter source, OleDbParameter target)
        //{
        //    target.IsNullable = source.IsNullable;
        //    target.OleDbType = source.OleDbType;
        //    target.Value = source.Value ?? DBNull.Value;
        //}

        //private static void CopyOdbcProps(OdbcParameter source, OdbcParameter target)
        //{
        //    target.IsNullable = source.IsNullable;
        //    target.OdbcType = source.OdbcType;
        //    target.Value = source.Value ?? DBNull.Value;
        //}

    }
}


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace Ockham.Data
{

    public static partial class DataExtensions
    {
        public delegate void CopyParameterDelegate(DbParameter source, DbParameter target);
        public delegate DbParameter AddNullableDelegate(DbParameterCollection parameters, string parameterName, object parameterValue);
        public delegate string QuoteIdentifierDelegate(string identifier);

        private static class ExtensionDelegates
        {

            public static Dictionary<Type, CopyParameterDelegate> CopyParameter { get; private set; }
            public static Dictionary<Type, AddNullableDelegate> AddNullable { get; private set; }
            private static HashSet<Type> _AddNullableFailed = new HashSet<Type>();

            static ExtensionDelegates()
            {
                CopyParameter = new Dictionary<Type, CopyParameterDelegate>();
                AddNullable = new Dictionary<Type, AddNullableDelegate>(); 
            }
             
            internal static bool GenerateAddNullable(Type paramCollectionType)
            {
                if (_AddNullableFailed.Contains(paramCollectionType)) return false;
                try
                {
                    var lDelegate = CreateAddNullableDelegate(paramCollectionType);
                    ExtensionDelegates.AddNullable[paramCollectionType] = lDelegate;
                    return true;
                }
                catch
                {
                    _AddNullableFailed.Add(paramCollectionType);
                    return false;
                }
            }

            /// <summary>
            /// Automatically generate an AddNullable overload for the particular parameter collection type 
            /// by looking for an AddWithValue method on the collection type
            /// </summary> 
            /// <returns></returns>
            private static AddNullableDelegate CreateAddNullableDelegate(Type paramCollectionType)
            {
                Type[] lParamTypes = new[] { typeof(string), typeof(object) };
                var mAddWithValue = paramCollectionType.GetMethod("AddWithValue", lParamTypes);
                var lpParams = Expression.Parameter(typeof(DbParameterCollection), "params");
                var lpName = Expression.Parameter(typeof(string), "parameterName");
                var lpValue = Expression.Parameter(typeof(object), "parameterValue");

                // Creates the following lambda:
                // (params, parameterName, parameterValue) => ((TParameterCollection)params).AddWithValue(parameterName, parameterValue ?? DBNull.Value)

                var lLambda = Expression.Lambda<AddNullableDelegate>(
                    Expression.Call(
                        Expression.Convert(lpParams, paramCollectionType),
                        mAddWithValue,
                        lpName,
                        Expression.Coalesce(
                            lpValue,
                            Expression.Constant(DBNull.Value)
                        )
                    ),
                    lpParams, lpName, lpValue
                );
                return lLambda.Compile();
            }
        }
    }
}


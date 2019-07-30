using Ockham.Data.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using static Ockham.Data.Extensions.Delegates;

namespace Ockham.Data
{
    /// <summary>
    /// Extensions to <see cref="DbParameterCollection"/>
    /// </summary>
    public static class DbParameterExtensions
    {
        /// <summary>
        /// Create a new parameter of the same type as members of <paramref name="parameters"/>. This
        /// method only creates a parameter, it does not add it to the collection on which the method is called
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DbParameter CreateParameter(this DbParameterCollection parameters)
        {
            return Ockham.Data.Common.DbProviderFactories.GetFactory(parameters.GetType().Namespace).CreateParameter();
        }

        /// <summary>
        /// Add a named parameter to the collection. Null <paramref name="parameterValue"/> values are automatically 
        /// converted to <see cref="DBNull"/>. See also <see cref="AddNullable(DbParameterCollection, string, object)"/>
        /// </summary> 
        public static DbParameter AddWithValue(this DbParameterCollection parameters, string parameterName, object parameterValue)
        {
            return AddNullable(parameters, parameterName, parameterValue);
        }

        /// <summary>
        /// Add a named parameter to the collection. Null <paramref name="parameterValue"/> values are automatically 
        /// converted to <see cref="DBNull"/>.  
        /// </summary> 
        public static DbParameter AddNullable(this DbParameterCollection parameters, string parameterName, object parameterValue)
        {
            Type tParamCollection = parameters.GetType();
            string @namespace = tParamCollection.Namespace;

            if (ProviderExtensions.AddNullableParameter.TryGetDelegate(@namespace, out AddNullableParameter addNullable))
            {
                return addNullable(parameters, parameterName, parameterValue);
            }

            var factory = Ockham.Data.Common.DbProviderFactories.GetFactory(@namespace);
            if (null == factory) throw new NotImplementedException("No DbProviderFactory found for namespace " + tParamCollection.Namespace);

            var param = factory.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = parameterValue ?? DBNull.Value;
            parameters.Add(param);
            return param;
        }

        /// <summary>
        /// Copy all properties of the source parameter to the target parameter
        /// </summary>
        public static void CopyTo(this DbParameter source, DbParameter target)
        {
            string sourceNamespace = source.GetType().Namespace;
            string targetNamespace = target.GetType().Namespace;

            if (sourceNamespace == targetNamespace)
            {
                if (ProviderExtensions.CopyParameter.TryGetDelegate(sourceNamespace, out CopyParameter copyParameter))
                {
                    copyParameter(source, target);
                }
            }
            else
            {
                CopyCommonProps(source, target);
            }

            source.DbType = target.DbType;
            source.Value = target.Value ?? DBNull.Value;
        }

        /// <summary>
        /// Copy all properties that are present on the base <see cref="DbParameter"/> class
        /// </summary>
        public static void CopyCommonProps(DbParameter source, DbParameter target)
        {
            target.Direction = source.Direction;
            target.ParameterName = source.ParameterName;
            target.Precision = source.Precision;
            target.Scale = source.Precision;
            target.Size = source.Size;
            target.SourceColumn = source.SourceColumn;
            target.SourceColumnNullMapping = source.SourceColumnNullMapping;
            target.SourceVersion = source.SourceVersion;
        }

        /// <summary>
        /// Copy all parameters from the <paramref name="source"/> <see cref="DbParameterCollection"/>, 
        /// using <see cref="CopyTo(DbParameter, DbParameter)"/>
        /// </summary>
        public static void AddParameters(this DbParameterCollection parameters, DbParameterCollection source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var param = parameters.CreateParameter();
                source[i].CopyTo(param);
                parameters.Add(param);
            }
        }

        /// <summary>
        /// Copy all parameters from the <paramref name="source"/> enumerable of <see cref="DbParameter"/>,
        /// using <see cref="CopyTo(DbParameter, DbParameter)"/>
        /// </summary>
        public static void AddParameters(this DbParameterCollection parameters, IEnumerable<DbParameter> source)
        {
            foreach (var sourceParam in source)
            {
                var param = parameters.CreateParameter();
                sourceParam.CopyTo(param);
                parameters.Add(param);
            }
        }

        /// <summary>
        /// Add the items in the provided <paramref name="dictionary"/> as parameters 
        /// </summary>
        public static void AddParameters(this DbParameterCollection parameters, IDictionary<string, object> dictionary)
        {
            foreach (var entry in dictionary)
            {
                parameters.AddNullable(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Add the items in the provided <paramref name="dictionary"/> as parameters 
        /// </summary>
        public static void AddParameters(this DbParameterCollection parameters, System.Collections.IDictionary dictionary)
        {
            foreach (object key in dictionary.Keys)
            {
                parameters.AddNullable(System.Convert.ToString(key), dictionary[key]);
            }
        }

        /// <summary>
        /// Add name and value of each column in the provided <paramref name="row"/> as a parameter
        /// </summary>
        public static void AddParameters(this DbParameterCollection parameters, DataRow row)
        {
            foreach (DataColumn c in row.Table.Columns)
            {
                parameters.AddNullable(c.ColumnName, row[c]);
            }
        }

        /// <summary>
        /// Add data from <paramref name="source"/> as parameters to the parameter collection. Uses 
        /// any matching overload of AddParameters if applicable (such as <see cref="AddParameters(DbParameterCollection, DbParameterCollection)"/>,
        /// and falls back to enumerating plain object property values with <see cref="TypeDescriptor"/>
        /// </summary>
        public static void AddParameters(this DbParameterCollection parameters, object source)
        {
            if (source == null) return;

            if (source is DbParameterCollection dbParameterCollection)
            {
                AddParameters(parameters, dbParameterCollection);
            }
            else if (source is IEnumerable<DbParameter> enumerable)
            {
                AddParameters(parameters, enumerable);
            }
            else
            {
                // Read the appropriate keys and values from the source 
                if (source is IDictionary<string, object> dict)
                {
                    AddParameters(parameters, dict);
                }
                else if (source is System.Collections.IDictionary basicDict)
                {
                    AddParameters(parameters, basicDict);
                }
                else if (source is DataRow row)
                {
                    AddParameters(parameters, row);
                }
                else
                {
                    // Fallback: Enumerate props using TypeDescriptor
                    foreach (PropertyDescriptor lProp in TypeDescriptor.GetProperties(source))
                    {
                        parameters.AddNullable(lProp.Name, lProp.GetValue(source));
                    }
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Ockham.Data.Common
{
#if HAS_DBPROVIDERFACTORIES
    /// <summary>
    /// Augmentation of <see cref="System.Data.Common.DbProviderFactories"/>. Guarantees case-insensitive matching (as in original .NET
    /// Framework implementation). Also provides utilities to normalize a providerInvariantName (namespace), or throw an exception 
    /// if a requested factory cannot be found.
    /// </summary>
#else
    /// <summary>
    /// Replacement for System.Data.Common.DbProviderFactories (not included in netstandard). Guarantees case-insensitive matching (as in original .NET
    /// Framework implementation). Also provides utilities to normalize a providerInvariantName (namespace), or throw an exception 
    /// if a requested factory cannot be found.
    /// </summary>
#endif
    public static class DbProviderFactories
    {
        /// <summary>
        /// Normalize the encoding, whitespace, and capitalization of the provided providerInvariantName (namespace)
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static string NormalizeNamespace(string @namespace)
        {
            if (string.IsNullOrWhiteSpace(@namespace)) return null;
            string lcNamespace = @namespace.Trim().ToLowerInvariant().Normalize();
            var parts = lcNamespace.Split('.');
            string result = string.Join(".", parts.Select(s => s.Substring(0, 1).ToUpperInvariant() + s.Substring(1)));
            result = result.Replace("client", "Client");
            result = result.Replace("db", "Db");
            return result;
        }

        private static readonly Dictionary<string, DbProviderFactory> _factories =
            new Dictionary<string, DbProviderFactory>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Get the appropriate <see cref="DbProviderFactory"/> for the provided <paramref name="connection"/>, 
        /// optionally throwing an exception if no factory is available
        /// </summary>
        public static DbProviderFactory GetFactory(DbConnection connection, bool errorIfNull)
        {
            var factory = GetFactory(connection);
            if (errorIfNull && factory == null)
            {
                throw new NotImplementedException("No DbProviderFactory found for namespace " + NormalizeNamespace(connection.GetType().Namespace));
            }
            return factory;
        }

        /// <summary>
        /// Get the appropriate <see cref="DbProviderFactory"/> for the provided <paramref name="connection"/>
        /// </summary>
        /// <remarks>Returns null if no factory is found</remarks>
        public static DbProviderFactory GetFactory(DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

#if HAS_DBPROVIDERFACTORIES
            // Attempt to get directly from connection using framework implementation
            var factoryFromConnection = System.Data.Common.DbProviderFactories.GetFactory(connection);
            if (factoryFromConnection != null) return factoryFromConnection;
#endif
            // Fall back to getting by the namespace of the connection's type
            return GetFactory(connection.GetType().Namespace);
        }

        /// <summary>
        /// Get the appropriate <see cref="DbProviderFactory"/> for the provided <paramref name="providerInvariantName"/> (namespace), 
        /// optionally throwing an exception if no factory is available
        /// </summary>
        public static DbProviderFactory GetFactory(string providerInvariantName, bool errorIfNull)
        {
            var factory = GetFactory(providerInvariantName);
            if (errorIfNull && factory == null)
            {
                throw new NotImplementedException("No DbProviderFactory found for namespace " + providerInvariantName);
            }
            return factory;
        }

        /// <summary>
        /// Get the appropriate <see cref="DbProviderFactory"/> for the provided <paramref name="providerInvariantName"/> (namespace)
        /// </summary>
        /// <remarks>Returns null if no factory is found</remarks>
        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            string @namespace = NormalizeNamespace(providerInvariantName);
            if (_factories.TryGetValue(@namespace, out DbProviderFactory factory)) return factory;

            lock (_factories)
            {
                if (_factories.TryGetValue(@namespace, out factory)) return factory;

#if HAS_DBPROVIDERFACTORIES
                factory = System.Data.Common.DbProviderFactories.GetFactory(providerInvariantName);
                if (factory != null)
                {
                    RegisterFactory(@namespace, factory);
                    return factory;
                }
                else
                {
                    // Case insensitive match
                    var data = System.Data.Common.DbProviderFactories.GetFactoryClasses();
                    var row = data.Rows.Cast<DataRow>().FirstOrDefault(
                        r => NormalizeNamespace(Convert.ToString(r["InvariantName"])) == @namespace
                    );
                    if (row != null)
                    {
                        string typeFullName = Convert.ToString(row["AssemblyQualifiedName"]);
                        Type factoryType = Type.GetType(typeFullName);
                        if (factoryType != null) RegisterFactory(@namespace, factoryType);
                    }
                }

                factory = System.Data.Common.DbProviderFactories.GetFactory(providerInvariantName);
                if (factory != null)
                {
                    RegisterFactory(@namespace, factory);
                    return factory;
                }
#endif
            }

            return null;
        }

        /// <summary>
        /// Register the factory class for the provided <paramref name="providerInvariantName"/> (namespace) by
        /// fully qualified type name. Will throw an exception if the type cannot be immediately resolved.
        /// </summary>
        public static void RegisterFactory(string providerInvariantName, string factoryTypeAssemblyQualifiedName)
        {
            if (null == NormalizeNamespace(providerInvariantName)) throw new ArgumentNullException(nameof(providerInvariantName));
            if (string.IsNullOrWhiteSpace(factoryTypeAssemblyQualifiedName)) throw new ArgumentNullException(nameof(factoryTypeAssemblyQualifiedName));
            Type factoryType = Type.GetType(factoryTypeAssemblyQualifiedName);
            if (factoryType == null) throw new ArgumentException($"Type name {factoryTypeAssemblyQualifiedName} couldn't be loaded");

            RegisterFactory(providerInvariantName, factoryType);
        }

        /// <summary>
        /// Register a factory class for the provided <paramref name="providerInvariantName"/> (namespace)  
        /// </summary>
        public static void RegisterFactory(string providerInvariantName, Type providerFactoryClass)
        {
            if (null == providerFactoryClass) throw new ArgumentNullException(nameof(providerFactoryClass));

            if (!typeof(DbProviderFactory).IsAssignableFrom(providerFactoryClass))
                throw new InvalidCastException($"Provided type ${providerFactoryClass} is not a {nameof(DbProviderFactory)}");

            var factory = GetInstance(providerFactoryClass);
            if (factory == null)
            {
                throw new InvalidOperationException($"Could not create or retrieve instance of {providerFactoryClass.FullName}");
            }

            RegisterFactory(providerInvariantName, factory);
        }

        /// <summary>
        /// Register a factory instance for the provided <paramref name="providerInvariantName"/> (namespace)  
        /// </summary>
        public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
        {
            string @namespace = NormalizeNamespace(providerInvariantName);
            lock (_factories)
            {
                if (_factories.ContainsKey(@namespace) && _factories[@namespace].GetType() == factory.GetType())
                {
                    // Same class as already registered
                    return;
                }

                // Replace
                _factories[@namespace] = factory;
            }
        }

        private static DbProviderFactory GetInstance(Type providerFactoryClass)
        {
            var ps_Instance = providerFactoryClass.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if ((ps_Instance != null) && typeof(DbProviderFactory).IsAssignableFrom(ps_Instance.PropertyType))
            {
                return (DbProviderFactory)ps_Instance.GetValue(null);
            }

            return (DbProviderFactory)Activator.CreateInstance(providerFactoryClass);
        }

        private static Dictionary<Type, DbCommandBuilder> _CommandBuilders = new Dictionary<Type, DbCommandBuilder>();

        internal static DbCommandBuilder GetSharedCommandBuilder(Type connectionType)
        {
            if (_CommandBuilders.TryGetValue(connectionType, out DbCommandBuilder commandBuilder)) return commandBuilder;
            lock (_CommandBuilders)
            {
                if (_CommandBuilders.TryGetValue(connectionType, out commandBuilder)) return commandBuilder;
                return (_CommandBuilders[connectionType] = GetFactory(connectionType.Namespace, true)?.CreateCommandBuilder());
            }
        }

    }
}
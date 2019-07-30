using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ockham.Reflection;
using Ockham.RdbInternal;

using static Ockham.Data.Common.DbProviderFactories;


namespace Ockham.Data.Extensions
{

    /// <summary>
    /// Loads and indexes available exension methods by provider invariant name (namespace), such as "System.Data.SqlClient" or "MySql.Data.MySqlClient"
    /// </summary>
    public static partial class ProviderExtensions
    {
        /// <summary>
        /// The <see cref="ExtensionLoader"/> instance for the <see cref="ExtensionUris.Scheme"/> extension scheme
        /// </summary>
        public static ExtensionLoader Loader { get; } = ExtensionLoader.ForUri(ExtensionUris.Scheme);

        /// <summary>
        /// Try to get the applicable extension method for the namespace of the type of the provided <paramref name="connection"/>
        /// </summary>
        public static bool TryGetExtension<TDelegate>(DbConnection connection, out TDelegate @delegate) where TDelegate : Delegate
            => TryGetExtension(connection.GetType().Name, out @delegate);

        /// <summary>
        /// Try to get the applicable extension method for specified <paramref name="namespace"/>
        /// </summary>
        public static bool TryGetExtension<TDelegate>(string @namespace, out TDelegate @delegate) where TDelegate : Delegate
        {
            @delegate = null;
            @namespace = NormalizeNamespace(@namespace) ?? throw new ArgumentNullException(nameof(@namespace));
            Type tDelegate = typeof(TDelegate);

            if (!_extensions.TryGetValue(tDelegate, out IExtensionSet extensionSet))
            {
                //throw new KeyNotFoundException($"Type {tDelegate.FullName} is not a recognized extension delegate type");
                return false;
            }

            if (null == (@delegate = (extensionSet.GetDelegate(@namespace) as TDelegate)))
            {
                //throw new KeyNotFoundException($"No {tDelegate.Name} extension found for namespace {@namespace}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load the extension methods from the specified assembly
        /// </summary>
        /// <param name="assembly"></param>
        public static void LoadAssembly(System.Reflection.Assembly assembly)
        {
            ExtensionLoader.ScanAssembly(assembly);
        }

        /// <summary>
        /// Index any classes and methods already loaded, and register handlers to detect any that are loaded later
        /// </summary>
        static ProviderExtensions()
        {
            LoadClasses();
            LoadMethods();

            Loader.ClassLoaded += Loader_ClassLoaded;
            Loader.MethodLoaded += Loader_MethodLoaded;
        }

        internal static (string @namespace, string memberName) ParseNamespace(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return (null, null);
            var parts = source.Trim().Split('.');
            if (parts.Length < 3) return (NormalizeNamespace(source), null);

            var @namespace = string.Join(".", parts.Take(parts.Length - 1));
            return (NormalizeNamespace(@namespace), parts.Last());
        }

        private static void LoadClasses()
            => Loader.GetExtensionClasses<DbProviderFactory>().InvokeAll(c => LoadClass(c.ImplementingType, c.TargetType, c.ClassUri));

        private static void Loader_ClassLoaded(object sender, ClassLoadedEventArgs e)
            => LoadClass(e.ImplementingType, e.TargetType, e.ClassUri);

        private static void LoadClass(Type implementingType, Type targetType, string classUri)
        {
            if (typeof(DbProviderFactory).IsAssignableFrom(targetType) && (implementingType != null))
            {
                Ockham.Data.Common.DbProviderFactories.RegisterFactory(classUri ?? implementingType.Namespace, implementingType);
            }
        }

        private static void LoadMethods()
            => Loader.GetExtensionMethods().InvokeAll(m => LoadMethod(m.Delegate, m.TargetType, m.MethodUri));

        private static void Loader_MethodLoaded(object sender, MethodLoadedEventArgs e)
            => LoadMethod(e.ImplementingMethod, e.TargetDelegateType, e.MethodUri);

        private static void LoadMethod(Delegate @delegate, Type targetType, string methodUri)
        {
            // Method uri is treated as provided namespace
            string @namespace = NormalizeNamespace(methodUri);

            Type tDelegate;

            if (targetType != null)
            {
                // Source attribute bound to specific delegate type
                tDelegate = targetType;

                // Remove the trailing method name, if included
                // E.g. "System.Data.SqlClient.FillTable" --> "System.Data.SqlClient"
                if (methodUri.ToLower().EndsWith(tDelegate.Name.ToLower()))
                {
                    (@namespace, _) = ParseNamespace(methodUri);
                }

                if (!_extensions.TryGetValue(tDelegate, out IExtensionSet extensionSet))
                {
                    throw new KeyNotFoundException($"Type {tDelegate.FullName} is not a recognized extension delegate type");
                }
                extensionSet.AddDelegate(@namespace, @delegate);
                return;
            }

            if (@namespace == null)
            {
                throw new ArgumentException(
                    $"Invalid ExtensionMethodAttribute on method {@delegate.Method.ToString()} has neither a method uri nor a target delegate type"
                );
            }

            // Need to parse out namespace and method name from the method uri
            string methodName;
            (@namespace, methodName) = ParseNamespace(methodUri);

            tDelegate = Delegates.GetDelegate(methodName);
            if (tDelegate == null)
            {
                // One last try: Get from name of actual delegate method
                string implementingMethodName = @delegate.Method.Name;
                tDelegate = Delegates.GetDelegate(implementingMethodName);
                if (tDelegate == null)
                {
                    // Not found
                    throw new ArgumentException($"{methodName} is not a recognized extension method name");
                }
                else
                {
                    // Use the full method uri as the namespace
                    @namespace = NormalizeNamespace(methodUri);
                }
            }

            _extensions[tDelegate].AddDelegate(@namespace, @delegate);
        }

        private static readonly Dictionary<Type, IExtensionSet> _extensions =
            new Dictionary<Type, IExtensionSet>()
            {
                { typeof(Delegates.AddNullableParameter), new ExtensionSet<Delegates.AddNullableParameter>() },
                { typeof(Delegates.CopyParameter), new ExtensionSet<Delegates.CopyParameter>() },
                { typeof(Delegates.ExecuteProcedure), new ExtensionSet<Delegates.ExecuteProcedure>() },
                { typeof(Delegates.FillTable), new ExtensionSet<Delegates.FillTable>() },
                { typeof(Delegates.QuoteIdentifier), new ExtensionSet<Delegates.QuoteIdentifier>() }
            };

    }
}

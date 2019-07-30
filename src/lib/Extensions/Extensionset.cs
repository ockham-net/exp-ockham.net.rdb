using Ockham.RdbInternal;
using System;
using System.Collections.Generic;
using static Ockham.Data.Common.DbProviderFactories;

namespace Ockham.Data.Extensions
{
    /*
    private interface IExtensionSet
    {
        Type DelegateType { get; }

        Delegate GetDelegate(string @namespace);
        void AddDelegate(string @namespace, Delegate @delegate);
    }
    */

    /// <summary>
    /// A basic map of normalized namespace to extension delegate. The keys are the 
    /// provider invariant names (namespaces) for the ADO provider, such as "System.Data.SqlClient",
    /// or "MySql.Data.MySqlClient". The values are the implementation of the corresponding 
    /// delegate, such as <see cref="Delegates.FillTable"/>
    /// </summary>
    /// <typeparam name="TDelegate">The delegate type of an extension method. One of <see cref="Delegates" /></typeparam>
    public class ExtensionSet<TDelegate>   /*: IExtensionSet*/ where TDelegate : Delegate
    {
        internal ExtensionSet() { }

        /// <summary>
        /// The <see cref="Type"/> of <typeparamref name="TDelegate"/>
        /// </summary>
        public Type DelegateType => typeof(TDelegate);

        /// <summary>
        /// Register an extension delegate for the specified namespace / providerInvariantName
        /// </summary>
        public void Add(string @namespace, TDelegate @delegate)
        {
            @namespace = NormalizeNamespace(@namespace) ?? throw new ArgumentNullException(nameof(@namespace));
            _map[@namespace] = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
        }

        /// <summary>
        /// Retrieve the extension delegate, if any, for the specified namespace / providerInvariantName.
        /// Returns null if no exension is registered
        /// </summary>
        public TDelegate GetDelegate(string @namespace)
        {
            @namespace = NormalizeNamespace(@namespace) ?? throw new ArgumentNullException(nameof(@namespace));
            return _map.ItemOrDefault(@namespace);
        }

        /// <summary>
        /// Attempt to retrieve the extension delegate, if any, for the specified namespace / providerInvariantName.
        /// Returns true if a delegate was found, false if not
        /// </summary>
        public bool TryGetDelegate(string @namespace, out TDelegate @delegate)
        {
            @namespace = NormalizeNamespace(@namespace) ?? throw new ArgumentNullException(nameof(@namespace));
            return _map.TryGetValue(@namespace, out @delegate);
        }

        /// <summary>
        /// Attempt to retrieve the extension delegate, if any, for specified namespace / providerInvariantName
        /// that matches the namespace of the type of <paramref name="connection"/>.
        /// Returns true if a delegate was found, false if not
        /// </summary>
        public bool TryGetDelegate(System.Data.Common.DbConnection connection, out TDelegate @delegate)
            => TryGetDelegate(connection.GetType().Namespace, out @delegate);

        private readonly Dictionary<string, TDelegate> _map = new Dictionary<string, TDelegate>(StringComparer.OrdinalIgnoreCase);

        /*
        #region IExtensionSet

        Delegate IExtensionSet.GetDelegate(string @namespace) => GetDelegate(@namespace);

        void IExtensionSet.AddDelegate(string @namespace, Delegate @delegate)
        {
            if (!(@delegate is TDelegate))
            {
                try
                {
                    @delegate = @delegate.Method.CreateDelegate(typeof(TDelegate));
                }
                catch
                {
                    throw new InvalidCastException($"Cannot convert provided delegate of type {@delegate.GetType().FullName} to target delegate type {typeof(TDelegate).FullName}");
                }
            }
            Add(@namespace, @delegate as TDelegate);
        }

        #endregion
        */
    }
}

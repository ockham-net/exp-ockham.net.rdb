using Ockham.RdbInternal;
using Ockham.Reflection;
using System;
using System.Collections.Generic;
using static Ockham.Data.Common.DbProviderFactories;

namespace Ockham.Data.Extensions
{
    public static partial class ProviderExtensions
    {
        private interface IExtensionSet
        {
            Type DelegateType { get; }

            Delegate GetDelegate(string @namespace);
            void AddDelegate(string @namespace, Delegate @delegate);
        }

        /// <summary>
        /// A basic map of normalized namespace to extension delegate. The keys are the 
        /// provider invariant names (namespaces) for the ADO provider, such as "System.Data.SqlClient",
        /// or "MySql.Data.MySqlClient". The values are the implementation of the corresponding 
        /// delegate, such as <see cref="Delegates.FillTable"/>
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type of an extension method. One of <see cref="Delegates" /></typeparam>
        private class ExtensionSet<TDelegate> : IExtensionSet where TDelegate : Delegate
        {
            private readonly Dictionary<string, TDelegate> _map = new Dictionary<string, TDelegate>(StringComparer.OrdinalIgnoreCase);

            public ExtensionSet() { }

            public Type DelegateType => typeof(TDelegate);

            public void Add(string @namespace, TDelegate @delegate)
            {
                _map[NormalizeNamespace(@namespace)] = @delegate;
            }

            public Delegate GetDelegate(string @namespace) => _map.ItemOrDefault(NormalizeNamespace(@namespace));

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
        }
    }
}

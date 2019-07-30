using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ockham.Reflection
{
    public static class ExtensionHelper
    {
        private static List<Action<Assembly>> _AssemblyLoaders;
        private static object lockObj = new object();

        static ExtensionHelper()
        {
            _AssemblyLoaders = new List<Action<Assembly>>();
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            HandleAssembly(args.LoadedAssembly);
        }

        private static void HandleAssembly(Assembly assembly)
        {
            lock (lockObj)
            {
                foreach (var fnHandler in _AssemblyLoaders)
                {
                    fnHandler(assembly);
                }
            }
        }

        /// <summary>
        /// Register a handler that will be called for each assembly already loaded, and will
        /// be called when any new assemblies are loaded for the life of the process
        /// </summary>
        /// <param name="loader"></param>
        public static void AddAssemblyLoader(Action<Assembly> loader)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            lock (lockObj)
            {
                foreach (Assembly lAsm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    loader(lAsm);
                }
                _AssemblyLoaders.Add(loader);
            }
        }
    }
}

using Ockham.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ockham.Data.Tests
{
    public class RdbTest
    {
        [Fact]
        public void TestLoader()
        {
            Ockham.Reflection.ExtensionLoader.ScanAssembly(typeof(SqlClient.SqlConnectionExtensions).Assembly);
            bool found = ProviderExtensions.TryGetExtension("System.Data.SqlClient", out Delegates.ExecuteProcedure @delegate);

            Assert.True(found);
            Assert.NotNull(@delegate);
        }
    }
}

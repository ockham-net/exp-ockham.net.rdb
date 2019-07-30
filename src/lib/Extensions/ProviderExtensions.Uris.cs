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
    public static partial class ProviderExtensions
    { 
        public class ExtensionUris
        {
            public const string Scheme = nameof(Ockham) + "." + nameof(Ockham.Data) + "." + nameof(ProviderExtensions);

            public class Methods
            {
                public const string ExecuteProcedure = nameof(Delegates.ExecuteProcedure);
                public const string FillTable = nameof(Delegates.FillTable);
                public const string CopyParameter = nameof(Delegates.CopyParameter);
                public const string AddNullableParameter = nameof(Delegates.AddNullableParameter);
                public const string QuoteIdentifier = nameof(Delegates.QuoteIdentifier);
            }
        }

    }
}

using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Ockham.Data
{

    public static partial class DbConnectionExtensions
    {
        public static string QuoteIdentifier(this DbConnection connection, string identifier)
        {
            return (GetQuoteIdentifierDelegate(connection))(identifier);
        }

        public static string QuoteIdentifier(this DbConnection connection, params string[] identifierParts)
        {
            var fnQuote = GetQuoteIdentifierDelegate(connection);
            return QuoteIdentifier(fnQuote, identifierParts);
        }

        public static string QuoteIdentifier(Func<string, string> fnQuote, params string[] identifierParts)
        {
            return string.Join(".", identifierParts.Where(s => !string.IsNullOrEmpty(s)).Select(fnQuote));
        }


        /// <summary>
        /// Quote the provider identifier u
        /// </summary>
        /// <param name="source"></param>
        /// <param name="nameDelimiter">The opening or closing name delimiter. A matching delimiter is chosen, unless <paramref name="closeDelimiter"/> is explicitly provided</param>
        /// <param name="partDelimiter">The delimiter between identifier parts</param>
        /// <param name="force">True to treat the entire <paramref name="source"/> as a literal to be escaped. Otherwise, existing delimiters are left as is</param>
        /// <param name="closeDelimiter"></param>
        /// <returns></returns>
        public static string QuoteIdentifier(
            string source,
            char nameDelimiter = '"',
            char partDelimiter = '.',
            bool force = false,
            char? closeDelimiter = null)
        {
            throw null;
        }

        public static string[] ParseIdentifier(
            string source,
            char nameDelimiter = '"',
            char partDelimiter = '.',
            char? closeDelimiter = null)
        {

            throw null;
        }

        public static Func<string, string> GetQuoteIdentifierDelegate(this DbConnection connection)
        {
            var commandBuilder = Ockham.Data.Common.DbProviderFactories.GetSharedCommandBuilder(connection.GetType());
            if (null != commandBuilder)
            {
                if (!string.IsNullOrEmpty(commandBuilder.QuotePrefix) && !string.IsNullOrEmpty(commandBuilder.QuoteSuffix))
                {
                    // Builder provided by factory already has identifier delimiters
                    return identifier => commandBuilder.QuoteIdentifier(identifier);
                }
            }

            // Attempt to get the identifier delimiters from the data source
            DataRow lSourceRow = connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation).Rows[0];
            string lDelimPattern = Convert.ToString(lSourceRow[DbMetaDataColumnNames.QuotedIdentifierPattern]);
            string lProductame = Convert.ToString(lSourceRow[DbMetaDataColumnNames.DataSourceProductName]).ToLower();
            string lPrefix = string.Empty;
            string lSuffix = string.Empty;

            // First try to match some known patterns, then check for product names
            if (lDelimPattern == @"""^(([^""]|"""")*)$""" || lProductame.Contains("oracle"))
            {
                lPrefix = lSuffix = "\"";
            }
            else if (
                lDelimPattern == @"(([^\[]|\]\])*)" ||
                lDelimPattern == @"`(([^`]|``)*)`" ||
                lProductame.Contains("jet") ||
                lProductame.Contains("ace") ||
                lProductame.Contains("sql server"))
            {
                lPrefix = "[";
                lSuffix = "]";
            }
            else
            {
                // Fall back to ANSI standard double quote "
                lPrefix = lSuffix = "\"";
            }

            return identifier => lPrefix + (identifier ?? string.Empty) + lSuffix;

        }


    }
}

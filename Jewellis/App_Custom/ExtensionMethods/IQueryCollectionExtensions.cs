using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Jewellis
{
    /// <summary>
    /// Represents extension methods for <see cref="IQueryCollection"/>.
    /// </summary>
    public static class IQueryCollectionExtensions
    {

        /// <summary>
        /// Converts the query collection to a dictionary.
        /// </summary>
        /// <param name="queryCollection">This query collection to convert.</param>
        /// <returns>Returns the converted dictionary.</returns>
        public static IDictionary<string, string> ToDictionary(this IQueryCollection queryCollection)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var queryParam in queryCollection)
            {
                dictionary.Add(queryParam.Key, queryParam.Value);
            }
            return dictionary;
        }

    }
}

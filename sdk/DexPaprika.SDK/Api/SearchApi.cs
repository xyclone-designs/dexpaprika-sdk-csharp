using DexPaprika.SDK.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// API service for search-related endpoints.
    /// </summary>
    public class SearchApi(DexPaprikaClient client) : BaseApi(client)
    {
        /// <summary>
        /// Searches for tokens, pools, and DEXes by name or identifier.
        /// </summary>
        /// <param name="query">Search term (e.g., "uniswap", "bitcoin", or a token address).</param>
        /// <returns>Search results across tokens, pools, and DEXes.</returns>
        public Task<SearchResult> SearchAsync(string query, SearchOptions? _ = null)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["query"] = query
            };

            return GetAsync<SearchResult>("/search", queryParams);
        }

        public new class Options : BaseApi.Options { }
        public class SearchOptions() : Options
        {
            public new static readonly SearchOptions _Default = new();
            public new static class Defaults { }
        }
    }
}

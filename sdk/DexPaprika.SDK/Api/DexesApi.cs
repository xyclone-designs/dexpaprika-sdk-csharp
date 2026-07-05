using DexPaprika.SDK.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// API service for DEX-related endpoints.
    /// </summary>
    public class DexesApi(DexPaprikaClient client) : BaseApi(client)
    {
        /// <summary>
        /// Gets a list of DEXes on a specific network.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="page">Page number (default: 0).</param>
        /// <param name="limit">Number of results per page (default: 10).</param>
        /// <returns>Paginated response containing a list of DEXes on the network.</returns>
        public Task<DexPaginatedResponse> ListByNetworkAsync(string networkId, ListByNetworkOptions? options = null)
        {
            options ??= ListByNetworkOptions._Default;

            var queryParams = new Dictionary<string, object>
            {
                [ListByNetworkOptions.Params.Page] = options.Page,
                [ListByNetworkOptions.Params.Limit] = options.Limit
            };

            return GetAsync<DexPaginatedResponse>($"/networks/{networkId}/dexes", queryParams);
        }

        public new class Options : BaseApi.Options { }
        public class ListByNetworkOptions : Options
        {
            public new static readonly ListByNetworkOptions _Default = new();
            public new static class Defaults
            {
                public const int Limit = 10;
                public const int Page = 1;
            }
            public new static class Params
            {
                public const string Limit = "limit";
                public const string Page = "page";
            }

            public int Limit { get; set; } = Defaults.Limit;
            public int Page { get; set; } = Defaults.Page;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DexPaprika.SDK.Models;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// API service for token-related endpoints.
    /// </summary>
    public class TokensApi(DexPaprikaClient client) : BaseApi(client)
    {
        /// <summary>
        /// Filters tokens on a network by volume, liquidity, FDV, transactions, and creation date.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="options">Filter criteria and pagination options.</param>
        /// <returns>Filtered tokens with pagination info.</returns>
        public async Task<TokenFilterPaginatedResponse> FilterAsync(string networkId, FilterOptions? options = null)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["page"] = options?.Page ?? 1,
                ["limit"] = options?.Limit ?? 10,
                ["sort_by"] = options?.SortBy ?? "volume_24h",
                ["sort_dir"] = options?.SortDir ?? "desc"
            };

            if (options?.Volume24hMin.HasValue == true) queryParams["volume_24h_min"] = options.Volume24hMin.Value;
            if (options?.Volume24hMax.HasValue == true) queryParams["volume_24h_max"] = options.Volume24hMax.Value;
            if (options?.LiquidityUsdMin.HasValue == true) queryParams["liquidity_usd_min"] = options.LiquidityUsdMin.Value;
            if (options?.FdvMin.HasValue == true) queryParams["fdv_min"] = options.FdvMin.Value;
            if (options?.FdvMax.HasValue == true) queryParams["fdv_max"] = options.FdvMax.Value;
            if (options?.Txns24hMin.HasValue == true) queryParams["txns_24h_min"] = options.Txns24hMin.Value;
            if (options?.CreatedAfter != null) queryParams["created_after"] = options.CreatedAfter;
            if (options?.CreatedBefore != null) queryParams["created_before"] = options.CreatedBefore;

            // The token filter endpoint wraps rows in a "data" key; remap to "results"
            // to match the shape of other filter endpoints.
            var raw = await GetAsync<TokenFilterRawResponse>($"/networks/{networkId}/tokens/filter", queryParams);

            return new TokenFilterPaginatedResponse
            {
                Results = raw.Data ?? [],
                PageInfo = raw.PageInfo
            };
        }

        /// <summary>
        /// Gets detailed information about a specific token on a network.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="tokenAddress">Token address or identifier.</param>
        /// <returns>Detailed information about the token.</returns>
        public Task<TokenDetails> GetDetailsAsync(string networkId, string tokenAddress)
        {
            return GetAsync<TokenDetails>($"/networks/{networkId}/tokens/{tokenAddress}");
        }

        /// <summary>
        /// Gets batch prices for multiple tokens on a network.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="tokens">Array of token addresses (max 10).</param>
        /// <returns>Array of token prices.</returns>
        public Task<List<TokenPrice>> GetMultiPricesAsync(string networkId, IReadOnlyList<string> tokens)
        {
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("tokens must not be null or empty.", nameof(tokens));
            if (tokens.Count > 10)
                throw new ArgumentException("tokens must contain at most 10 addresses.", nameof(tokens));

            var queryParams = new Dictionary<string, object>
            {
                ["tokens"] = string.Join(",", tokens)
            };

            return GetAsync<List<TokenPrice>>($"/networks/{networkId}/multi/prices", queryParams);
        }

        /// <summary>
        /// Gets a list of top liquidity pools for a specific token on a network.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="tokenAddress">Token address or identifier.</param>
        /// <param name="page">Page number (default: 0).</param>
        /// <param name="limit">Number of results per page (default: 10).</param>
        /// <param name="sort">Sort direction: "asc" or "desc" (default: "desc").</param>
        /// <param name="orderBy">Field to order by (default: "volume_usd").</param>
        /// <param name="pairWith">Optional address of a token to filter pair pools.</param>
        /// <returns>Paginated list of pools that include the specified token.</returns>
        public Task<PoolPaginatedResponse> GetPoolsAsync(string networkId, string tokenAddress, GetPoolsOptions? options = null)
        {
            options ??= GetPoolsOptions._Default;

            var queryParams = new Dictionary<string, object>
            {
                ["page"] = options.Page,
                ["limit"] = options.Limit,
                ["sort"] = options.Sort,
                ["order_by"] = options.OrderBy
            };

            if (options.PairWith is not null) queryParams["address"] = options.PairWith;

            return GetAsync<PoolPaginatedResponse>($"/networks/{networkId}/tokens/{tokenAddress}/pools", queryParams);
        }

        /// <summary>
        /// Gets top tokens on a network ranked by volume, price, liquidity, or other metrics.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="page">Page number (default: 1).</param>
        /// <param name="limit">Number of results per page (default: 10).</param>
        /// <param name="orderBy">Field to order by (default: "volume_24h").</param>
        /// <param name="sort">Sort direction: "asc" or "desc" (default: "desc").</param>
        /// <returns>Top tokens with pagination info.</returns>
        public Task<TopTokensPaginatedResponse> GetTopAsync(string networkId, GetTopOptions? options = null)
        {
            options ??= GetTopOptions._Default;

            var queryParams = new Dictionary<string, object>
            {
                ["page"] = options.Page,
                ["limit"] = options.Limit,
                ["order_by"] = options.OrderBy,
                ["sort"] = options.Sort
            };

            return GetAsync<TopTokensPaginatedResponse>($"/networks/{networkId}/tokens/top", queryParams);
        }

        public new class Options : BaseApi.Options { }

        public class FilterOptions : BaseApi.Options
        {
            public new static readonly FilterOptions _Default = new();
            public new static class Defaults
            {
                public const int Page = 1;
                public const int Limit = 10;
                public const string SortBy = "volume_24h";
                public const string SortDir = "desc";
            }

            /// <summary>Page number (1-indexed).</summary>
            public int Page { get; set; } = Defaults.Page;
            /// <summary>Number of results per page (max 100).</summary>
            public int Limit { get; set; } = Defaults.Limit;
            /// <summary>Field to sort by.</summary>
            public string SortBy { get; set; } = Defaults.SortBy;
            /// <summary>Sort direction: "asc" or "desc".</summary>
            public string SortDir { get; set; } = Defaults.SortDir;
            /// <summary>Minimum 24h volume in USD.</summary>
            public double? Volume24hMin { get; set; }
            /// <summary>Maximum 24h volume in USD.</summary>
            public double? Volume24hMax { get; set; }
            /// <summary>Minimum liquidity in USD.</summary>
            public double? LiquidityUsdMin { get; set; }
            /// <summary>Minimum fully diluted valuation in USD.</summary>
            public double? FdvMin { get; set; }
            /// <summary>Maximum fully diluted valuation in USD.</summary>
            public double? FdvMax { get; set; }
            /// <summary>Minimum 24h transaction count.</summary>
            public int? Txns24hMin { get; set; }
            /// <summary>Only include tokens created after this Unix timestamp (or ISO string).</summary>
            public object? CreatedAfter { get; set; }
            /// <summary>Only include tokens created before this Unix timestamp (or ISO string).</summary>
            public object? CreatedBefore { get; set; }
        }
        public class ListByNetworkOptions : Options
        {
            public new static readonly ListByNetworkOptions _Default = new();
            public new static class Defaults
            {
                public const int Page = 1;
                public const int Limit = 10;
                public const string Sort = "desc";
                public const string OrderBy = "volume_usd";
            }

            public int Page { get; set; } = Defaults.Page;
            public int Limit { get; set; } = Defaults.Limit;
            public string Sort { get; set; } = Defaults.Sort;
            public string OrderBy { get; set; } = Defaults.OrderBy;
        }
        public class GetPoolsOptions : Options
        {
            public new static readonly GetPoolsOptions _Default = new();
            public new static class Defaults
            {
                public const int Limit = 10;
                public const string OrderBy = "volume_usd";
                public const int Page = 0;
                public const string Sort = "desc";
            }

            public int Limit { get; set; } = Defaults.Limit;
            public string OrderBy { get; set; } = Defaults.OrderBy;
            public string? PairWith { get; set; }
            public int Page { get; set; } = Defaults.Page;
            public string Sort { get; set; } = Defaults.Sort;
        }
        public class GetTopOptions : Options
        {
            public new static readonly GetTopOptions _Default = new();
            public new static class Defaults
            {
                public const int Limit = 10;
                public const string OrderBy = "volume_24h";
                public const int Page = 1;
                public const string Sort = "desc";
            }

            public int Limit { get; set; } = Defaults.Limit;
            public string OrderBy { get; set; } = Defaults.OrderBy;
            public int Page { get; set; } = Defaults.Page;
            public string Sort { get; set; } = Defaults.Sort;
        }
    }
}

using DexPaprika.SDK.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                [FilterOptions.Params.Page] = options?.Page ?? FilterOptions.Defaults.Page,
                [FilterOptions.Params.Limit] = options?.Limit ??  FilterOptions.Defaults.Limit,
                [FilterOptions.Params.SortBy] = options?.SortBy ??  FilterOptions.Defaults.SortBy,
                [FilterOptions.Params.SortDir] = options?.SortDir ??  FilterOptions.Defaults.SortDir
            };

            if (options?.Volume24hMin.HasValue == true) queryParams[FilterOptions.Params.Volume24hMin] = options.Volume24hMin.Value;
            if (options?.Volume24hMax.HasValue == true) queryParams[FilterOptions.Params.Volume24hMax] = options.Volume24hMax.Value;
            if (options?.LiquidityUsdMin.HasValue == true) queryParams[FilterOptions.Params.LiquidityUsdMin] = options.LiquidityUsdMin.Value;
            if (options?.FdvMin.HasValue == true) queryParams[FilterOptions.Params.FdvMin] = options.FdvMin.Value;
            if (options?.FdvMax.HasValue == true) queryParams[FilterOptions.Params.FdvMax] = options.FdvMax.Value;
            if (options?.Txns24hMin.HasValue == true) queryParams[FilterOptions.Params.Txns24hMin] = options.Txns24hMin.Value;
            if (options?.CreatedAfter != null) queryParams[FilterOptions.Params.CreatedAfter] = options.CreatedAfter;
            if (options?.CreatedBefore != null) queryParams[FilterOptions.Params.CreatedBefore] = options.CreatedBefore;

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
            return GetMultiPricesAsync(networkId, new GetMultiPricesOptions(tokens));
        }
        /// <summary>
        /// Gets batch prices for multiple tokens on a network.
        /// </summary>
        /// <param name="networkId">Network ID (e.g., "ethereum", "solana").</param>
        /// <param name="tokens">Array of token addresses (max 10).</param>
        /// <returns>Array of token prices.</returns>
        public Task<List<TokenPrice>> GetMultiPricesAsync(string networkId, GetMultiPricesOptions options)
        {
            var queryParams = new Dictionary<string, object>
            {
                [GetMultiPricesOptions.Params.Tokens] = string.Join(",", options.Tokens)
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
                [GetPoolsOptions.Params.Page] = options.Page,
                [GetPoolsOptions.Params.Limit] = options.Limit,
                [GetPoolsOptions.Params.Sort] = options.Sort,
                [GetPoolsOptions.Params.OrderBy] = options.OrderBy
            };

            if (options.PairWith is not null) queryParams[GetPoolsOptions.Params.PairWith] = options.PairWith;

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
                [GetTopOptions.Params.Page] = options.Page,
                [GetTopOptions.Params.Limit] = options.Limit,
                [GetTopOptions.Params.OrderBy] = options.OrderBy,
                [GetTopOptions.Params.Sort] = options.Sort
            };

            return GetAsync<TopTokensPaginatedResponse>($"/networks/{networkId}/tokens/top", queryParams);
        }

        public new class Options : BaseApi.Options 
        {
            public new static class Params { }
        }

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
            public new static class Params
            {
                public const string Page = "page";
                public const string Limit = "limit";
                public const string SortBy = "sort_by";
                public const string SortDir = "sort_dir";

                public const string Volume24hMin = "volume_24h_min";
                public const string Volume24hMax = "volume_24h_max";
                public const string LiquidityUsdMin = "liquidity_usd_min";
                public const string FdvMin = "fdv_min";
                public const string FdvMax = "fdv_max";
                public const string Txns24hMin = "txns_24h_min";
                public const string CreatedAfter = "created_after";
                public const string CreatedBefore = "created_before";
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
        public class GetMultiPricesOptions : Options
        {
            public new static readonly GetPoolsOptions _Default = new();
            public new static class Defaults { }
            public new static class Params
            {
                public const string Tokens = "tokens";
            }

            public GetMultiPricesOptions(params string[] tokens) : this(tokens as IEnumerable<string>) { }
            public GetMultiPricesOptions(IEnumerable<string> tokens)
            {
                int count = tokens.Count();

                if (count == 0)
                    throw new ArgumentException("tokens must not be empty.", nameof(tokens));
                if (count > 10)
                    throw new ArgumentException("tokens must contain at most 10 addresses.", nameof(tokens));

                Tokens = [.. tokens];
            }

            public IReadOnlyList<string> Tokens { get; set; }
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
            public new static class Params
            {
                public const string Page = "page";
                public const string Limit = "limit";
                public const string Sort = "sort";
                public const string OrderBy = "order_by";            
                public const string PairWith = "address";            
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
            public new static class Params
            {
                public const string Page = "page";
                public const string Limit = "limit";
                public const string OrderBy = "order_by";
                public const string Sort = "sort";
            }

            public int Limit { get; set; } = Defaults.Limit;
            public string OrderBy { get; set; } = Defaults.OrderBy;
            public int Page { get; set; } = Defaults.Page;
            public string Sort { get; set; } = Defaults.Sort;
        }
    }
}

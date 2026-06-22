using DexPaprika.SDK.Models;
using DexPaprika.SDK.Utils;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// API service for pool-related endpoints.
    /// </summary>
    public class PoolsApi(DexPaprikaClient client) : BaseApi(client)
    {
        /// <summary>
        /// Filters pools on a network by volume, liquidity, transactions, and creation date.
        /// </summary>
        /// <param name="networkId">Network identifier.</param>
        /// <param name="options">Filter criteria and pagination options.</param>
        /// <returns>Filtered pools with pagination info.</returns>
        public Task<PoolFilterPaginatedResponse> FilterAsync(string networkId, FilterOptions? options = null)
        {
            options ??= FilterOptions._Default;

            if (string.IsNullOrEmpty(networkId))
                throw new ArgumentException("Network ID is required.", nameof(networkId));

            Dictionary<string, object> queryParams = new ()
            {
                ["page"] = options.Page,
                ["limit"] = options.Limit,
                ["sort_by"] = options.SortBy,
                ["sort_dir"] = options.SortDir
            };

            if (options.Volume24hMin.HasValue) queryParams["volume_24h_min"] = options.Volume24hMin.Value;
            if (options.Volume24hMax.HasValue) queryParams["volume_24h_max"] = options.Volume24hMax.Value;
            if (options.Volume7dMin.HasValue) queryParams["volume_7d_min"] = options.Volume7dMin.Value;
            if (options.Volume7dMax.HasValue) queryParams["volume_7d_max"] = options.Volume7dMax.Value;
            if (options.LiquidityUsdMin.HasValue) queryParams["liquidity_usd_min"] = options.LiquidityUsdMin.Value;
            if (options.LiquidityUsdMax.HasValue) queryParams["liquidity_usd_max"] = options.LiquidityUsdMax.Value;
            if (options.Txns24hMin.HasValue) queryParams["txns_24h_min"] = options.Txns24hMin.Value;
            if (options.CreatedAfter is not null) queryParams["created_after"] = options.CreatedAfter;
            if (options.CreatedBefore is not null) queryParams["created_before"] = options.CreatedBefore;

            return GetAsync<PoolFilterPaginatedResponse>($"/networks/{networkId}/pools/filter", queryParams);
        }

        /// <summary>
        /// Gets top pools across all networks.
        /// </summary>
        /// <remarks>
        /// DEPRECATED since API v1.3.0. Use <see cref="ListByNetworkAsync"/> instead.
        /// This endpoint returns a 410 Gone error.
        /// </remarks>
        /// <exception cref="DeprecatedEndpointException">Always thrown.</exception>
        [Obsolete("This endpoint is deprecated. Use ListByNetworkAsync(networkId, ...) instead.")]
        public Task<PoolPaginatedResponse> ListAsync()
        {
            throw new DeprecatedEndpointException(
                "/pools",
                "client.Pools.ListByNetworkAsync(network) — specify a network like 'ethereum', 'solana', or 'fantom'");
        }
        /// <summary>
        /// Gets pools on a specific DEX within a network.
        /// </summary>
        /// <param name="networkId">Network identifier (e.g., "ethereum", "solana").</param>
        /// <param name="dexId">DEX identifier (e.g., "uniswap_v2", "sushiswap").</param>
        /// <param name="page">Page number (default: 0).</param>
        /// <param name="limit">Number of results per page (default: 10).</param>
        /// <param name="sort">Sort direction: "asc" or "desc" (default: "desc").</param>
        /// <param name="orderBy">Field to order by (default: "volume_usd").</param>
        /// <returns>Paginated list of pools on the specified DEX.</returns>
        public Task<PoolPaginatedResponse> ListByDexAsync(string networkId, string dexId, ListByOptions? options = null)
        {
            options ??= new ListByOptions();

            if (string.IsNullOrEmpty(networkId))
                throw new ArgumentException("Network ID is required.", nameof(networkId));
            if (string.IsNullOrEmpty(dexId))
                throw new ArgumentException("DEX ID is required.", nameof(dexId));

            var queryParams = new Dictionary<string, object>
            {
                ["page"] = options.Page,
                ["limit"] = options.Limit,
                ["sort"] = options.Sort,
                ["order_by"] = options.OrderBy
            };

            return GetAsync<PoolPaginatedResponse>($"/networks/{networkId}/dexes/{dexId}/pools", queryParams);
        }
        /// <summary>
        /// Gets pools on a specific network with pagination and sorting.
        /// </summary>
        /// <param name="networkId">Network identifier (e.g., "ethereum", "solana").</param>
        /// <param name="page">Page number (default: 0).</param>
        /// <param name="limit">Number of results per page (default: 10).</param>
        /// <param name="sort">Sort direction: "asc" or "desc" (default: "desc").</param>
        /// <param name="orderBy">Field to order by (default: "volume_usd").</param>
        /// <returns>Paginated list of pools on the specified network.</returns>
        public Task<PoolPaginatedResponse> ListByNetworkAsync(string networkId, ListByOptions? options = null)
        {
            options ??= new ListByOptions();

            if (string.IsNullOrEmpty(networkId))
                throw new ArgumentException("Network ID is required. Use 'ethereum', 'solana', 'fantom', etc.", nameof(networkId));

            var queryParams = new Dictionary<string, object>
            {
                ["page"] = options.Page,
                ["limit"] = options.Limit,
                ["sort"] = options.Sort,
                ["order_by"] = options.OrderBy
            };

            return GetAsync<PoolPaginatedResponse>($"/networks/{networkId}/pools", queryParams);
        }

        /// <summary>
        /// Gets detailed information about a specific pool.
        /// </summary>
        /// <param name="networkId">Network identifier.</param>
        /// <param name="poolAddress">On-chain address of the pool.</param>
        /// <param name="inversed">Whether to invert the price ratio (default: false).</param>
        /// <returns>Detailed pool information.</returns>
        public Task<PoolDetails> GetDetailsAsync(string networkId, string poolAddress, GetDetailsOptions? options = null)
        {
            options ??= new GetDetailsOptions();

            if (string.IsNullOrEmpty(networkId))
                throw new ArgumentException("Network ID is required.", nameof(networkId));
            if (string.IsNullOrEmpty(poolAddress))
                throw new ArgumentException("Pool address is required.", nameof(poolAddress));

            Dictionary<string, object> queryParams = new ()
            {
                { "inversed", options.Inversed }
            };

            return GetAsync<PoolDetails>($"/networks/{networkId}/pools/{poolAddress}", queryParams);
        }
        /// <summary>
        /// Gets OHLCV (Open-High-Low-Close-Volume) data for a pool.
        /// </summary>
        /// <param name="networkId">Network identifier.</param>
        /// <param name="poolAddress">On-chain address of the pool.</param>
        /// <param name="start">Start of the time range.</param>
        /// <param name="limit">Number of records (default: 1).</param>
        /// <param name="interval">Time interval (default: "24h").</param>
        /// <param name="end">Optional end of the time range.</param>
        /// <param name="inversed">Whether to invert the price ratio (default: false).</param>
        /// <returns>Time-series OHLCV data.</returns>
        public Task<List<OhlcvRecord>> GetOhlcvAsync(string networkId, string poolAddress, string start, GetOhlcvOptions? options = null)
        {
            options ??= new GetOhlcvOptions();

            if (string.IsNullOrEmpty(networkId))
                throw new ArgumentException("Network ID is required.", nameof(networkId));
            if (string.IsNullOrEmpty(poolAddress))
                throw new ArgumentException("Pool address is required.", nameof(poolAddress));

            Dictionary<string, object> queryParams = new()
            {
                { "start", start },
                { "limit", options.Limit },
                { "interval", options.Interval },
                { "inversed", options.Inversed }
            };

            if (options.End != null) queryParams["end"] = options.End;

            return GetAsync<List<OhlcvRecord>>($"/networks/{networkId}/pools/{poolAddress}/ohlcv", queryParams);
        }
        /// <summary>
        /// Gets transaction history for a specific pool.
        /// </summary>
        /// <param name="networkId">Network identifier.</param>
        /// <param name="poolAddress">On-chain address of the pool.</param>
        /// <param name="page">Page number (default: 0).</param>
        /// <param name="limit">Number of results per page (default: 10).</param>
        /// <param name="cursor">Optional pagination cursor.</param>
        /// <param name="from">Optional timestamp lower bound.</param>
        /// <param name="to">Optional timestamp upper bound.</param>
        /// <returns>List of pool transactions.</returns>
        public Task<PoolTransactionsResponse> GetTxsAsync(string networkId, string poolAddress, GetTxsOptions? options = null)
        {
            options ??= GetTxsOptions._Default;

            if (string.IsNullOrEmpty(networkId))
                throw new ArgumentException("Network ID is required.", nameof(networkId));
            if (string.IsNullOrEmpty(poolAddress))
                throw new ArgumentException("Pool address is required.", nameof(poolAddress));

            var queryParams = new Dictionary<string, object>
            {
                ["page"] = options.Page,
                ["limit"] = options.Limit
            };

            if (options.Cursor != null) queryParams["cursor"] = options.Cursor;
            if (options.From.HasValue) queryParams["from"] = options.From.Value;
            if (options.To.HasValue) queryParams["to"] = options.To.Value;

            return GetAsync<PoolTransactionsResponse>($"/networks/{networkId}/pools/{poolAddress}/transactions", queryParams);
        }
        /// <summary>
        /// Alias for <see cref="GetTxsAsync"/>.
        /// </summary>
        public Task<PoolTransactionsResponse> GetTransactionsAsync(string networkId, string poolAddress, GetTransactionsOptions? options = null)
        {
            return GetTxsAsync(networkId, poolAddress, options);
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
            /// <summary>Minimum 7d volume in USD.</summary>
            public double? Volume7dMin { get; set; }
            /// <summary>Maximum 7d volume in USD.</summary>
            public double? Volume7dMax { get; set; }
            /// <summary>Minimum liquidity in USD.</summary>
            public double? LiquidityUsdMin { get; set; }
            /// <summary>Maximum liquidity in USD.</summary>
            public double? LiquidityUsdMax { get; set; }
            /// <summary>Minimum 24h transaction count.</summary>
            public int? Txns24hMin { get; set; }
            /// <summary>Only include pools created after this Unix timestamp (or ISO string).</summary>
            public object? CreatedAfter { get; set; }
            /// <summary>Only include pools created before this Unix timestamp (or ISO string).</summary>
            public object? CreatedBefore { get; set; }
        }
        public class ListByOptions : Options
        {
            public new static readonly ListByOptions _Default = new();
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
        public class GetDetailsOptions : Options
        {
            public new static readonly GetDetailsOptions _Default = new();
            public new static class Defaults 
            {
                public const bool Inversed = false;
            }

            public bool Inversed { get; set; } = Defaults.Inversed;
        }
        public class GetOhlcvOptions : Options
        {
            public new static readonly GetOhlcvOptions _Default = new();
            public new static class Defaults 
            {
                public const int Limit = 1;
                public const string Interval = "24h";
                public const bool Inversed = false;
            }

            public string? End { get; set; }
            public int Limit { get; set; } = Defaults.Limit;
            public string Interval { get; set; } = Defaults.Interval;
            public bool Inversed { get; set; } = Defaults.Inversed;
        }
        public class GetTxsOptions : Options
        {
            public new static readonly GetTxsOptions _Default = new();
            public new static class Defaults 
            {
                public const int Limit = 10;
                public const int Page = 1;
            }

            public string? Cursor { get; set; }
            public long? From { get; set; }
            public int Limit { get; set; } = Defaults.Limit;
            public int Page { get; set; } = Defaults.Page;
            public long? To { get; set; }
        }
        public class GetTransactionsOptions : GetTxsOptions { }
    }
}

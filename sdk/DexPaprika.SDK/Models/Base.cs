using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// Pagination metadata returned alongside paginated responses.
    /// </summary>
    public class PageInfo
    {
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>Total number of items. May be absent in some responses.</summary>
        [JsonPropertyName("total_items")]
        public int? TotalItems { get; set; }

        /// <summary>Total number of pages. May be absent in some responses.</summary>
        [JsonPropertyName("total_pages")]
        public int? TotalPages { get; set; }

        /// <summary>Cursor for the next page in cursor-based pagination.</summary>
        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }
    }

    /// <summary>
    /// Generic paginated response wrapper.
    /// </summary>
    public class PaginatedResponse<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = [];

        [JsonPropertyName("page_info")]
        public PageInfo PageInfo { get; set; } = new();
    }

    /// <summary>
    /// Paginated response containing a list of DEXes.
    /// </summary>
    public class DexPaginatedResponse
    {
        [JsonPropertyName("dexes")]
        public List<Dex> Dexes { get; set; } = [];

        [JsonPropertyName("page_info")]
        public PageInfo PageInfo { get; set; } = new();
    }

    /// <summary>
    /// Paginated response containing a list of pools.
    /// </summary>
    public class PoolPaginatedResponse
    {
        [JsonPropertyName("pools")]
        public List<Pool> Pools { get; set; } = [];

        [JsonPropertyName("page_info")]
        public PageInfo PageInfo { get; set; } = new();
    }

    /// <summary>
    /// Paginated response from the pool filter endpoint (uses "results" key).
    /// </summary>
    public class PoolFilterPaginatedResponse
    {
        [JsonPropertyName("results")]
        public List<FilteredPool> Results { get; set; } = [];

        [JsonPropertyName("page_info")]
        public PageInfo PageInfo { get; set; } = new();
    }
}

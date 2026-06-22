using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// DEX entry returned within search results.
    /// </summary>
    public class DexResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("pools_count")]
        public int PoolsCount { get; set; }
    }

    /// <summary>
    /// Aggregated search results across tokens, pools, and DEXes.
    /// </summary>
    public class SearchResult
    {
        [JsonPropertyName("tokens")]
        public List<Token> Tokens { get; set; } = [];

        [JsonPropertyName("pools")]
        public List<Pool> Pools { get; set; } = [];

        [JsonPropertyName("dexes")]
        public List<DexResult> Dexes { get; set; } = [];
    }
}

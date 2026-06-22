using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// Decentralized exchange information.
    /// </summary>
    public class Dex
    {
        /// <summary>DEX identifier.</summary>
        [JsonPropertyName("dex_id")]
        public string DexId { get; set; } = string.Empty;

        /// <summary>DEX name.</summary>
        [JsonPropertyName("dex_name")]
        public string DexName { get; set; } = string.Empty;

        /// <summary>Network the DEX is on.</summary>
        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        /// <summary>Protocol type.</summary>
        [JsonPropertyName("protocol")]
        public string? Protocol { get; set; }

        /// <summary>DEX website URL.</summary>
        [JsonPropertyName("website")]
        public string? Website { get; set; }

        /// <summary>DEX explorer URL.</summary>
        [JsonPropertyName("explorer")]
        public string? Explorer { get; set; }

        /// <summary>Twitter URL.</summary>
        [JsonPropertyName("twitter")]
        public string? Twitter { get; set; }

        /// <summary>Discord URL.</summary>
        [JsonPropertyName("discord")]
        public string? Discord { get; set; }

        /// <summary>Telegram URL.</summary>
        [JsonPropertyName("telegram")]
        public string? Telegram { get; set; }

        /// <summary>Number of pools on the DEX.</summary>
        [JsonPropertyName("pools_count")]
        public int? PoolsCount { get; set; }
    }
}

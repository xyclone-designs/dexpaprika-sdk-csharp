using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// Blockchain network information.
    /// </summary>
    public class Network
    {
        /// <summary>Network identifier (e.g., "ethereum", "solana").</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>Human-readable name of the network.</summary>
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Network logo URL.</summary>
        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; set; }

        /// <summary>Number of DEXes on the network.</summary>
        [JsonPropertyName("dexes_count")]
        public int DexesCount { get; set; }
    }
}

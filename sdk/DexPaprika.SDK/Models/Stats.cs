using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// Ecosystem-wide statistics for DexPaprika.SDK.
    /// </summary>
    public class Stats
    {
        /// <summary>Number of chains tracked.</summary>
        [JsonPropertyName("chains")]
        public int Chains { get; set; }

        /// <summary>Number of DEX factories.</summary>
        [JsonPropertyName("factories")]
        public int Factories { get; set; }

        /// <summary>Number of pools.</summary>
        [JsonPropertyName("pools")]
        public int Pools { get; set; }

        /// <summary>Number of tokens.</summary>
        [JsonPropertyName("tokens")]
        public int Tokens { get; set; }
    }
}

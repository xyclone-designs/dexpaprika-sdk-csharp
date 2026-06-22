using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// Detailed token information (extends <see cref="Token"/>).
    /// </summary>
    public class TokenDetails : Token
    {
        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; set; }

        [JsonPropertyName("twitter")]
        public string? Twitter { get; set; }

        [JsonPropertyName("discord")]
        public string? Discord { get; set; }

        [JsonPropertyName("telegram")]
        public string? Telegram { get; set; }

        [JsonPropertyName("contract_address")]
        public string? ContractAddress { get; set; }

        [JsonPropertyName("market_cap")]
        public double? MarketCap { get; set; }

        [JsonPropertyName("price_usd")]
        public double? PriceUsd { get; set; }

        [JsonPropertyName("price_change_24h")]
        public double? PriceChange24h { get; set; }

        [JsonPropertyName("price_change_7d")]
        public double? PriceChange7d { get; set; }

        [JsonPropertyName("price_change_30d")]
        public double? PriceChange30d { get; set; }

        [JsonPropertyName("circulating_supply")]
        public double? CirculatingSupply { get; set; }

        [JsonPropertyName("market_data")]
        public TokenMarketData? MarketData { get; set; }

        [JsonPropertyName("links")]
        public TokenLinks? Links { get; set; }

        [JsonPropertyName("social_data")]
        public TokenSocialData? SocialData { get; set; }
    }

    /// <summary>
    /// Market data snapshot for a token.
    /// </summary>
    public class TokenMarketData
    {
        [JsonPropertyName("price_usd")]
        public double PriceUsd { get; set; }

        [JsonPropertyName("volume_usd_24h")]
        public double VolumeUsd24h { get; set; }

        [JsonPropertyName("price_change_24h")]
        public double PriceChange24h { get; set; }

        [JsonPropertyName("ath")]
        public double Ath { get; set; }

        [JsonPropertyName("ath_date")]
        public string AthDate { get; set; } = string.Empty;

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;
    }

    /// <summary>
    /// External links for a token.
    /// </summary>
    public class TokenLinks
    {
        [JsonPropertyName("twitter")]
        public string? Twitter { get; set; }

        [JsonPropertyName("telegram")]
        public string? Telegram { get; set; }

        [JsonPropertyName("github")]
        public string? Github { get; set; }

        [JsonPropertyName("coinmarketcap")]
        public string? CoinMarketCap { get; set; }
    }

    /// <summary>
    /// Social media statistics for a token.
    /// </summary>
    public class TokenSocialData
    {
        [JsonPropertyName("twitter_followers")]
        public int TwitterFollowers { get; set; }

        [JsonPropertyName("telegram_members")]
        public int TelegramMembers { get; set; }
    }

    /// <summary>
    /// Per-interval trading metrics for a token.
    /// </summary>
    public class TokenTimeIntervalMetrics
    {
        [JsonPropertyName("volume")]
        public double Volume { get; set; }

        [JsonPropertyName("volume_usd")]
        public double VolumeUsd { get; set; }

        [JsonPropertyName("buy_usd")]
        public double BuyUsd { get; set; }

        [JsonPropertyName("sell_usd")]
        public double SellUsd { get; set; }

        [JsonPropertyName("sells")]
        public int Sells { get; set; }

        [JsonPropertyName("buys")]
        public int Buys { get; set; }

        [JsonPropertyName("txns")]
        public int Txns { get; set; }

        [JsonPropertyName("last_price_usd_change")]
        public double? LastPriceUsdChange { get; set; }
    }

    /// <summary>
    /// Aggregated summary metrics for a token across all its pools.
    /// </summary>
    public class TokenSummary
    {
        [JsonPropertyName("price_usd")]
        public double PriceUsd { get; set; }

        [JsonPropertyName("fdv")]
        public double? Fdv { get; set; }

        [JsonPropertyName("liquidity_usd")]
        public double LiquidityUsd { get; set; }

        [JsonPropertyName("pools")]
        public int Pools { get; set; }

        [JsonPropertyName("24h")]
        public TokenTimeIntervalMetrics Metrics24h { get; set; } = new();

        [JsonPropertyName("6h")]
        public TokenTimeIntervalMetrics? Metrics6h { get; set; }

        [JsonPropertyName("1h")]
        public TokenTimeIntervalMetrics? Metrics1h { get; set; }

        [JsonPropertyName("30m")]
        public TokenTimeIntervalMetrics? Metrics30m { get; set; }

        [JsonPropertyName("15m")]
        public TokenTimeIntervalMetrics? Metrics15m { get; set; }

        [JsonPropertyName("5m")]
        public TokenTimeIntervalMetrics? Metrics5m { get; set; }

        [JsonPropertyName("1m")]
        public TokenTimeIntervalMetrics? Metrics1m { get; set; }
    }

    /// <summary>
    /// Lighter per-interval metrics returned by the top-tokens endpoint.
    /// </summary>
    public class TopTokenTimeMetrics
    {
        [JsonPropertyName("volume_usd")]
        public double VolumeUsd { get; set; }

        [JsonPropertyName("txns")]
        public int Txns { get; set; }

        [JsonPropertyName("last_price_usd_change")]
        public double? LastPriceUsdChange { get; set; }

        [JsonPropertyName("buys")]
        public int? Buys { get; set; }

        [JsonPropertyName("sells")]
        public int? Sells { get; set; }
    }

    /// <summary>
    /// Token entry returned by the top-tokens endpoint.
    /// </summary>
    public class TopToken
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("has_image")]
        public bool? HasImage { get; set; }

        [JsonPropertyName("price_usd")]
        public double? PriceUsd { get; set; }

        [JsonPropertyName("fdv")]
        public double? Fdv { get; set; }

        [JsonPropertyName("liquidity_usd")]
        public double? LiquidityUsd { get; set; }

        [JsonPropertyName("pools")]
        public int? Pools { get; set; }

        [JsonPropertyName("24h")]
        public TopTokenTimeMetrics? Metrics24h { get; set; }

        [JsonPropertyName("1h")]
        public TopTokenTimeMetrics? Metrics1h { get; set; }

        [JsonPropertyName("5m")]
        public TopTokenTimeMetrics? Metrics5m { get; set; }
    }

    /// <summary>
    /// Paginated response from the top-tokens endpoint.
    /// </summary>
    public class TopTokensPaginatedResponse
    {
        [JsonPropertyName("tokens")]
        public List<TopToken> Tokens { get; set; } = [];

        [JsonPropertyName("page_info")]
        public PageInfo PageInfo { get; set; } = new();
    }

    /// <summary>
    /// Token entry returned by the filter endpoint.
    /// </summary>
    public class FilteredToken
    {
        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("price_usd")]
        public double? PriceUsd { get; set; }

        [JsonPropertyName("volume_usd_24h")]
        public double? VolumeUsd24h { get; set; }

        [JsonPropertyName("volume_usd_7d")]
        public double? VolumeUsd7d { get; set; }

        [JsonPropertyName("volume_usd_30d")]
        public double? VolumeUsd30d { get; set; }

        [JsonPropertyName("liquidity_usd")]
        public double? LiquidityUsd { get; set; }

        [JsonPropertyName("fdv_usd")]
        public double? FdvUsd { get; set; }

        [JsonPropertyName("txns_24h")]
        public int? Txns24h { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }
    }

    /// <summary>
    /// Paginated response from the token filter endpoint.
    /// </summary>
    public class TokenFilterPaginatedResponse
    {
        public List<FilteredToken> Results { get; set; } = [];
        public PageInfo PageInfo { get; set; } = new();
    }

    /// <summary>
    /// Internal raw shape of the token filter response (API wraps rows in "data").
    /// Remapped to <see cref="TokenFilterPaginatedResponse"/> before returning to callers.
    /// </summary>
    public class TokenFilterRawResponse
    {
        [JsonPropertyName("data")]
        public List<FilteredToken>? Data { get; set; }

        [JsonPropertyName("page_info")]
        public PageInfo PageInfo { get; set; } = new();
    }

    /// <summary>
    /// Token price entry from the multi-prices endpoint.
    /// </summary>
    public class TokenPrice
    {
        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("price_usd")]
        public double? PriceUsd { get; set; }
    }

    // Alias kept for backward compatibility
    public class TokenFilterItem : FilteredToken { }
}

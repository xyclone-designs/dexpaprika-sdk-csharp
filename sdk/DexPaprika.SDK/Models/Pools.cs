using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DexPaprika.SDK.Models
{
    /// <summary>
    /// Basic token information embedded within pool responses.
    /// </summary>
    public class Token
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("added_at")]
        public string AddedAt { get; set; } = string.Empty;

        [JsonPropertyName("fdv")]
        public double? Fdv { get; set; }

        [JsonPropertyName("total_supply")]
        public double? TotalSupply { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("explorer")]
        public string? Explorer { get; set; }

        [JsonPropertyName("last_updated")]
        public string? LastUpdated { get; set; }

        [JsonPropertyName("summary")]
        public TokenSummary? Summary { get; set; }
    }

    /// <summary>
    /// Pool data returned from list endpoints.
    /// </summary>
    public class Pool
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("dex_id")]
        public string DexId { get; set; } = string.Empty;

        [JsonPropertyName("dex_name")]
        public string DexName { get; set; } = string.Empty;

        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("volume_usd")]
        public double VolumeUsd { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("created_at_block_number")]
        public long CreatedAtBlockNumber { get; set; }

        [JsonPropertyName("transactions")]
        public int Transactions { get; set; }

        [JsonPropertyName("price_usd")]
        public double PriceUsd { get; set; }

        [JsonPropertyName("last_price_change_usd_5m")]
        public double? LastPriceChangeUsd5m { get; set; }

        [JsonPropertyName("last_price_change_usd_1h")]
        public double? LastPriceChangeUsd1h { get; set; }

        [JsonPropertyName("last_price_change_usd_24h")]
        public double? LastPriceChangeUsd24h { get; set; }

        [JsonPropertyName("fee")]
        public double? Fee { get; set; }

        [JsonPropertyName("tokens")]
        public List<Token> Tokens { get; set; } = [];

        [JsonPropertyName("volume_usd_7d")]
        public double? VolumeUsd7d { get; set; }

        [JsonPropertyName("liquidity_usd")]
        public double? LiquidityUsd { get; set; }
    }

    /// <summary>
    /// Pool returned from the filter endpoint. Includes timeframe-split volumes
    /// instead of the flat <c>volume_usd</c> found on <see cref="Pool"/>.
    /// </summary>
    public class FilteredPool
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("dex_id")]
        public string DexId { get; set; } = string.Empty;

        [JsonPropertyName("dex_name")]
        public string DexName { get; set; } = string.Empty;

        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("tokens")]
        public List<Token> Tokens { get; set; } = [];

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("created_at_block_number")]
        public long? CreatedAtBlockNumber { get; set; }

        [JsonPropertyName("transactions")]
        public int? Transactions { get; set; }

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

        [JsonPropertyName("last_price_change_usd_5m")]
        public double? LastPriceChangeUsd5m { get; set; }

        [JsonPropertyName("last_price_change_usd_1h")]
        public double? LastPriceChangeUsd1h { get; set; }

        [JsonPropertyName("last_price_change_usd_24h")]
        public double? LastPriceChangeUsd24h { get; set; }

        [JsonPropertyName("fee")]
        public double? Fee { get; set; }
    }

    /// <summary>
    /// Metrics for a specific time interval on a pool or token.
    /// </summary>
    public class TimeIntervalMetrics
    {
        [JsonPropertyName("last_price_usd_change")]
        public double LastPriceUsdChange { get; set; }

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
    }

    /// <summary>
    /// Detailed pool information including per-interval metrics.
    /// </summary>
    public class PoolDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("created_at_block_number")]
        public long CreatedAtBlockNumber { get; set; }

        [JsonPropertyName("chain")]
        public string Chain { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("factory_id")]
        public string FactoryId { get; set; } = string.Empty;

        [JsonPropertyName("dex_id")]
        public string DexId { get; set; } = string.Empty;

        [JsonPropertyName("dex_name")]
        public string DexName { get; set; } = string.Empty;

        [JsonPropertyName("tokens")]
        public List<Token> Tokens { get; set; } = [];

        [JsonPropertyName("last_price")]
        public double LastPrice { get; set; }

        [JsonPropertyName("last_price_usd")]
        public double LastPriceUsd { get; set; }

        [JsonPropertyName("fee")]
        public double? Fee { get; set; }

        [JsonPropertyName("price_time")]
        public string PriceTime { get; set; } = string.Empty;

        [JsonPropertyName("24h")]
        public TimeIntervalMetrics Metrics24h { get; set; } = new();

        [JsonPropertyName("6h")]
        public TimeIntervalMetrics? Metrics6h { get; set; }

        [JsonPropertyName("1h")]
        public TimeIntervalMetrics? Metrics1h { get; set; }

        [JsonPropertyName("30m")]
        public TimeIntervalMetrics? Metrics30m { get; set; }

        [JsonPropertyName("15m")]
        public TimeIntervalMetrics? Metrics15m { get; set; }

        [JsonPropertyName("5m")]
        public TimeIntervalMetrics? Metrics5m { get; set; }
    }

    /// <summary>
    /// A single OHLCV (Open-High-Low-Close-Volume) data point.
    /// </summary>
    public class OhlcvRecord
    {
        [JsonPropertyName("time_open")]
        public string TimeOpen { get; set; } = string.Empty;

        [JsonPropertyName("time_close")]
        public string TimeClose { get; set; } = string.Empty;

        [JsonPropertyName("open")]
        public double Open { get; set; }

        [JsonPropertyName("high")]
        public double High { get; set; }

        [JsonPropertyName("low")]
        public double Low { get; set; }

        [JsonPropertyName("close")]
        public double Close { get; set; }

        [JsonPropertyName("volume")]
        public double Volume { get; set; }
    }

    /// <summary>
    /// A single pool transaction.
    /// </summary>
    public class Transaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("log_index")]
        public int LogIndex { get; set; }

        [JsonPropertyName("transaction_index")]
        public int TransactionIndex { get; set; }

        [JsonPropertyName("pool_id")]
        public string PoolId { get; set; } = string.Empty;

        [JsonPropertyName("sender")]
        public string Sender { get; set; } = string.Empty;

        /// <summary>Recipient address or numeric ID (varies by chain).</summary>
        [JsonPropertyName("recipient")]
        public object? Recipient { get; set; }

        [JsonPropertyName("token_0")]
        public string Token0 { get; set; } = string.Empty;

        [JsonPropertyName("token_1")]
        public string Token1 { get; set; } = string.Empty;

        /// <summary>Amount of the first token (may be string or number).</summary>
        [JsonPropertyName("amount_0")]
        public object? Amount0 { get; set; }

        /// <summary>Amount of the second token (may be string or number).</summary>
        [JsonPropertyName("amount_1")]
        public object? Amount1 { get; set; }

        [JsonPropertyName("created_at_block_number")]
        public long CreatedAtBlockNumber { get; set; }
    }

    /// <summary>
    /// Paginated response containing pool transactions.
    /// </summary>
    public class PoolTransactionsResponse : PaginatedResponse<Transaction>
    {
        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; } = [];
    }
}

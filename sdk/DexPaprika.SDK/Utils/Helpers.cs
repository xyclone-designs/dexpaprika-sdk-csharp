using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DexPaprika.SDK.Utils
{
    /// <summary>
    /// Miscellaneous formatting, parsing, and timing utilities.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Formats a currency value with a B / M / K suffix.
        /// </summary>
        public static string FormatVolume(double vol)
        {
            if (vol >= 1_000_000_000)
                return $"${vol / 1_000_000_000:F2}B";
            if (vol >= 1_000_000)
                return $"${vol / 1_000_000:F2}M";
            if (vol >= 1_000)
                return $"${vol / 1_000:F2}K";

            return $"${vol:F2}";
        }
        /// <summary>
        /// Returns a "TOKEN0/TOKEN1" display string for a trading pair.
        /// </summary>
        public static string FormatPair(string token0, string token1) => $"{token0}/{token1}";

        /// <summary>
        /// Converts a Unix timestamp (seconds) or ISO-8601 string to a <see cref="DateTime"/>.
        /// </summary>
        public static DateTime ParseDate(string input) => DateTime.Parse(input, null, System.Globalization.DateTimeStyles.RoundtripKind);
        /// <summary>
        /// Converts a Unix timestamp (seconds) to a UTC <see cref="DateTime"/>.
        /// </summary>
        public static DateTime ParseDate(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
        /// <summary>
        /// Extracts a human-readable message from an exception.
        /// </summary>
        public static string ParseError(Exception ex) => ex.Message ?? "Unknown error occurred";

        /// <summary>
        /// Current UTC time as a Unix timestamp (seconds).
        /// </summary>
        public static long Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        /// <summary>
        /// Unix timestamp for 24 hours ago.
        /// </summary>
        public static long Yesterday() => Now() - 86_400;
        /// <summary>
        /// Unix timestamp for 7 days ago.
        /// </summary>
        public static long LastWeek() => Now() - 86_400 * 7;

        /// <summary>
        /// Suspends execution for <paramref name="milliseconds"/> milliseconds.
        /// </summary>
        public static Task Sleep(int milliseconds) => Task.Delay(milliseconds);
    }
    /// <summary>
    /// Executes an async operation with configurable retry behaviour.
    /// </summary>
    public static class RetryHelper
    {
        private static readonly RetryConfig DefaultConfig = new();

        /// <summary>
        /// Runs <paramref name="operation"/>, retrying on transient failures according
        /// to <paramref name="config"/>.
        /// </summary>
        /// <typeparam name="T">Return type of the operation.</typeparam>
        /// <param name="operation">The async work to execute.</param>
        /// <param name="config">
        /// Retry settings. Omit to use <see cref="DefaultConfig"/>.
        /// </param>
        /// <returns>The result of the first successful invocation.</returns>
        /// <exception cref="Exception">Re-throws the last exception when all retries are exhausted.</exception>
        public static async Task<T> WithRetryAsync<T>(Func<Task<T>> operation, RetryConfig? config = null)
        {
            var cfg = config ?? DefaultConfig;
            Exception? lastException = null;

            for (int attempt = 0; attempt <= cfg.MaxRetries; attempt++)
            {
                // Wait before retry attempts (not before the first try)
                if (attempt > 0)
                {
                    var delayIndex = attempt - 1;
                    var delay = delayIndex < cfg.DelaySequenceMs.Count
                        ? cfg.DelaySequenceMs[delayIndex]
                        : cfg.DelaySequenceMs[cfg.DelaySequenceMs.Count - 1];

                    await Task.Delay(delay);
                }

                try
                {
                    return await operation();
                }
                catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
                {
                    // Only retry on configured status codes
                    if (!cfg.RetryableStatuses.Contains((int)ex.StatusCode.Value))
                        throw;

                    lastException = ex;

                    if (attempt == cfg.MaxRetries)
                        throw;
                }
                catch (Exception ex)
                {
                    // Retry network/timeout errors that have no status code
                    lastException = ex;

                    if (attempt == cfg.MaxRetries)
                        throw;
                }
            }

            // Unreachable — the loop always throws before falling through
            throw lastException!;
        }
    }
}

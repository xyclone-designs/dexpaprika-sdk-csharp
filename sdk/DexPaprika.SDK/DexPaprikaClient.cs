using DexPaprika.SDK.Api;
using DexPaprika.SDK.Utils;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DexPaprika.SDK
{
    /// <summary>
    /// Client configuration options.
    /// </summary>
    public class ClientConfig
    {
        /// <summary>Retry behaviour. Defaults to <see cref="RetryConfig"/> defaults.</summary>
        public RetryConfig? Retry { get; set; }
        /// <summary>Cache behaviour. Defaults to <see cref="CacheConfig"/> defaults.</summary>
        public CacheConfig? Cache { get; set; }
    }
    /// <summary>
    /// Configuration for <see cref="RetryHelper.WithRetryAsync{T}"/>.
    /// </summary>
    public class RetryConfig
    {
        /// <summary>
        /// Maximum number of retry attempts (not counting the initial call).
        /// </summary>
        public int MaxRetries { get; set; } = 4;
        /// <summary>
        /// Explicit delay in milliseconds before each retry attempt.
        /// The i-th retry uses <c>DelaySequenceMs[i-1]</c>; if the index is out of
        /// range the last element is used.
        /// </summary>
        public IReadOnlyList<int> DelaySequenceMs { get; set; } = [100, 500, 1000, 5000];
        /// <summary>
        /// HTTP status codes that should cause a retry.
        /// </summary>
        public IReadOnlyList<int> RetryableStatuses { get; set; } = [408, 429, 500, 502, 503, 504];
    } 

    /// <summary>
    /// Main entry point for the DexPaprika SDK.
    /// </summary>
    public class DexPaprikaClient : IDisposable
    {
        /// <summary>
        /// Creates a new <see cref="DexPaprikaClient"/>.
        /// </summary>
        /// <param name="baseUrl">API base URL (default: https://api.dexpaprika.com).</param>
        /// <param name="httpClient">
        /// Optional pre-configured <see cref="HttpClient"/>. When omitted a new instance
        /// is created and disposed with the client.
        /// </param>
        /// <param name="config">Retry and cache configuration.</param>
        public DexPaprikaClient(string baseUrl = "https://api.dexpaprika.com", HttpClient? httpClient = null, ClientConfig? config = null)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _retryConfig = config?.Retry ?? new RetryConfig();
            _cache = new Cache<object>(config?.Cache ?? new CacheConfig());

            HttpClient = httpClient ?? new HttpClient();
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "DexPaprika-SDK-CSharp/0.1.0");
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");

            Networks = new NetworksApi(this);
            Pools = new PoolsApi(this);
            Tokens = new TokensApi(this);
            Search = new SearchApi(this);
            Utils = new UtilsApi(this);
            Dexes = new DexesApi(this);
        }

        private static string BuildCacheKey(string endpoint, Dictionary<string, object>? queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return endpoint;

            return $"{endpoint}:{JsonConvert.SerializeObject(queryParams)}";
        }
        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            // Read the body to extract an error message if available
            string body = string.Empty;
            try { body = await response.Content.ReadAsStringAsync(); } catch { }

            string message = "Unknown API error";
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var errProp))
                    message = errProp.GetString() ?? message;
                else if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    message = msgProp.GetString() ?? message;
            }
            catch { /* body wasn't JSON */ }

            // Throw a typed HttpRequestException carrying the status code so
            // BaseApi and RetryHelper can inspect it.
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        private readonly string _baseUrl;
        private readonly RetryConfig _retryConfig;
        private readonly Cache<object> _cache;
        private bool _disposed;

        private string BuildUrl(string endpoint, Dictionary<string, object>? queryParams)
        {
            var url = $"{_baseUrl}{endpoint}";

            if (queryParams == null || queryParams.Count == 0)
                return url;

            var qs = HttpUtility.ParseQueryString(string.Empty);
            foreach (var (key, value) in queryParams)
                qs[key] = value?.ToString();

            return $"{url}?{qs}";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed is false)
            {
                if (disposing)
                {
                    _cache.Clear();
                    HttpClient.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>Network-related endpoints.</summary>
        public NetworksApi Networks { get; }
        /// <summary>Pool-related endpoints.</summary>
        public PoolsApi Pools { get; }
        /// <summary>Token-related endpoints.</summary>
        public TokensApi Tokens { get; }
        /// <summary>Search endpoint.</summary>
        public SearchApi Search { get; }
        /// <summary>Utility endpoints.</summary>
        public UtilsApi Utils { get; }
        /// <summary>DEX-related endpoints.</summary>
        public DexesApi Dexes { get; }

        /// <summary>Number of entries currently in the cache.</summary>
        public int CacheSize { get => _cache.Count; }
        public bool CacheEnabled { get => _cache.Enabled; set => _cache.Enabled = value; }
        public HttpClient HttpClient { get; }

        /// <summary>Clears all cached responses.</summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Makes a GET request with caching and retry.
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, object>? queryParams = null)
        {
            var cacheKey = BuildCacheKey(endpoint, queryParams);
            var cached = _cache.Get(cacheKey);

            if (cached is T hit)
                return hit;

            var result = await RetryHelper.WithRetryAsync(async () =>
            {
                var url = BuildUrl(endpoint, queryParams);
                var response = await HttpClient.GetAsync(url);

                await EnsureSuccessAsync(response);

                var json = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<T>(json) ?? throw new DexPaprikaException($"Empty response for GET {endpoint}");

            }, _retryConfig);

            _cache.Set(cacheKey, result!);

            return result;
        }
        /// <summary>
        /// Makes a POST request with retry (responses are not cached).
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, Dictionary<string, object> body, Dictionary<string, object>? queryParams = null)
        {
            return await RetryHelper.WithRetryAsync(async () =>
            {
                var url = BuildUrl(endpoint, queryParams);
                var json = JsonConvert.SerializeObject(body);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(url, content);
                
                await EnsureSuccessAsync(response);

                var responseJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(json) ?? throw new DexPaprikaException($"Empty response for POST {endpoint}");
            
            }, _retryConfig);
        }

        ~DexPaprikaClient()
        {
            Dispose(disposing: false);
        }
    }
}

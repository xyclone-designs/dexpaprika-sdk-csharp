using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DexPaprika.SDK.Utils;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// Base class for all API services.
    /// </summary>
    /// <remarks>
    /// Initializes a new API service.
    /// </remarks>
    /// <param name="client">DexPaprika client instance.</param>
    public abstract class BaseApi(DexPaprikaClient client)
    {
        private static string ExtractNetworkId(string endpoint, Dictionary<string, object>? queryParams)
        {
            if (queryParams != null && queryParams.TryGetValue("network", out var network))
                return network?.ToString() ?? "unknown";

            var match = System.Text.RegularExpressions.Regex.Match(endpoint, @"/networks/([^/]+)");
            return match.Success ? match.Groups[1].Value : "unknown";
        }

        protected readonly DexPaprikaClient Client = client ?? throw new ArgumentNullException(nameof(client));

        /// <summary>
        /// Makes a GET request with enhanced error handling.
        /// </summary>
        /// <typeparam name="T">Expected response type.</typeparam>
        /// <param name="endpoint">API endpoint path.</param>
        /// <param name="params">Optional query parameters.</param>
        /// <returns>Deserialized response data.</returns>
        /// <exception cref="DeprecatedEndpointException">If the endpoint returns 410 Gone.</exception>
        /// <exception cref="NetworkNotFoundException">If the network is not found.</exception>
        /// <exception cref="PoolNotFoundException">If the pool is not found.</exception>
        /// <exception cref="ApiException">For other API-level errors.</exception>
        /// <exception cref="DexPaprikaException">For general SDK errors.</exception>
        protected async Task<T> GetAsync<T>(string endpoint, Dictionary<string, object>? queryParams = null)
        {
            try
            {
                return await Client.GetAsync<T>(endpoint, queryParams);
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                var status = ex.StatusCode.Value;
                var message = ex.Message ?? "Unknown API error";

                if (status == HttpStatusCode.Gone) // 410
                {
                    if (endpoint == "/pools")
                        throw new DeprecatedEndpointException("/pools", "network-specific endpoints like client.Pools.ListByNetworkAsync(\"ethereum\")");

                    throw new DeprecatedEndpointException(endpoint, "check API documentation for alternatives");
                }

                if (status == HttpStatusCode.NotFound) // 404
                {
                    if (message.Contains("network", StringComparison.OrdinalIgnoreCase))
                    {
                        var networkId = ExtractNetworkId(endpoint, queryParams);
                        throw new NetworkNotFoundException(networkId);
                    }

                    if (message.Contains("pool", StringComparison.OrdinalIgnoreCase) || endpoint.Contains("/pools/"))
                    {
                        var poolAddress = endpoint.Split('/')[^1];
                        throw new PoolNotFoundException(poolAddress);
                    }
                }

                throw new ApiException(message, (int)status);
            }
            catch (Exception ex) when (ex is not DexPaprikaException)
            {
                throw new DexPaprikaException(ex.Message ?? "Unknown error occurred", ex);
            }
        }
        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <typeparam name="T">Expected response type.</typeparam>
        /// <param name="endpoint">API endpoint path.</param>
        /// <param name="body">Request body.</param>
        /// <param name="queryParams">Optional query parameters.</param>
        /// <returns>Deserialized response data.</returns>
        protected async Task<T> PostAsync<T>(string endpoint, Dictionary<string, object> body, Dictionary<string, object>? queryParams = null)
        {
            return await Client.PostAsync<T>(endpoint, body, queryParams);
        }

        public class Options 
        {
            public static readonly Options _Default = new();
            public static class Defaults { }
        }
    }
}

using System;

namespace DexPaprika.SDK.Utils
{
    /// <summary>
    /// Base exception for all DexPaprika SDK errors.
    /// </summary>
    public class DexPaprikaException : Exception
    {
        public DexPaprikaException(string message) : base(message) { }
        public DexPaprikaException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when a network ID is not found or invalid.
    /// </summary>
    public class NetworkNotFoundException(string networkId) : DexPaprikaException($"Network not found: {networkId}") { }

    /// <summary>
    /// Thrown when a pool address is not found.
    /// </summary>
    public class PoolNotFoundException(string poolAddress) : DexPaprikaException($"Pool not found: {poolAddress}") { }

    /// <summary>
    /// General API error that carries an HTTP status code.
    /// </summary>
    public class ApiException(string message, int statusCode) : DexPaprikaException($"API Error ({statusCode}): {message}")
    {
        /// <summary>HTTP status code returned by the API.</summary>
        public int StatusCode { get; } = statusCode;
    }

    /// <summary>
    /// Thrown when a caller attempts to use an endpoint that has been deprecated and removed.
    /// </summary>
    public class DeprecatedEndpointException(string endpoint, string alternative) : 
        DexPaprikaException(string.Format("The {0} endpoint has been deprecated and removed. Please use {1} instead. For more information, visit: {2}", endpoint, alternative, DexPaprikaURIs.DocsChangelog)) { }
}

using DexPaprika.SDK;
using DexPaprika.SDK.Utils;

using Moq;
using Moq.Protected;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public class RetryCacheTests
    {
        // Helper to simulate an HttpClient that fails a specific number of times
        private static HttpClient CreateMockHttpClient(int failureCount, out int attemptCountClosure)
        {
            int attempts = 0;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(() =>
                {
                    attempts++;
                    if (attempts <= failureCount)
                    {
                        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable); // 503
                    }
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{ \"success\": true }")
                    };
                });

            attemptCountClosure = 0; // Reference keeper
            // We use a custom wrapper or interceptor if we want to extract attempts, 
            // but for standard Moq we can verify via times or a local variable:
            return new HttpClient(handlerMock.Object);
        }

        #region Retry Tests

        [Fact]
        public async Task TestRetry_SuccessAfterTwoFailures()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            int attempts = 0;
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    attempts++;
                    if (attempts <= 2) return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{ 'success': true }") };
                });

            var client = new DexPaprikaClient(
                "https://example.com",
                new HttpClient(handlerMock.Object),
                new ClientConfig
                {
                    Retry = new RetryConfig { MaxRetries = 2, DelaySequenceMs = new[] { 100, 200, 300 } }
                }
            );

            // Act
            var result = await client.GetAsync<object>("/test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, attempts); // 2 failures + 1 success
        }

        [Fact]
        public async Task TestRetry_Exhaustion_AllAttemptsFail()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            int attempts = 0;
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    attempts++;
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                });

            var client = new DexPaprikaClient(
                "https://example.com",
                new HttpClient(handlerMock.Object),
                new ClientConfig 
                {
                    Retry = new RetryConfig { MaxRetries = 2, DelaySequenceMs = [100, 200] } 
                }                
            );

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync<object>("/test"));
            Assert.Equal(3, attempts); // Initial try + 2 retries
        }

        [Fact]
        public async Task TestRetry_WithRetryUtilityDirectly()
        {
            // Arrange
            int attempts = 0;
            var options = new RetryConfig { MaxRetries = 3, DelaySequenceMs = new[] { 100, 200, 300 } };

            // Act
            string result = await RetryHelper.WithRetryAsync(async () =>
            {
                attempts++;

                if (attempts <= 2)
                    throw new Exception("Simulated error");

                return await Task.FromResult("success");

            }, options);

            // Assert
            Assert.Equal("success", result);
            Assert.Equal(3, attempts);
        }

        #endregion

        #region Caching Tests

        [Fact]
        public void TestCaching_HitAndMiss()
        {
            // Arrange
            var cache = new Cache<string>(new CacheConfig { Ttl = TimeSpan.FromSeconds(5) });
            string key = "test-key";

            // Act & Assert - First access (Miss)
            bool isFound = cache.TryGetValue(key, out var value);
            Assert.False(isFound);
            Assert.Null(value);

            // Set Value
            cache.Set(key, "cached-value");

            // Second access (Hit)
            isFound = cache.TryGetValue(key, out value);
            Assert.True(isFound);
            Assert.Equal("cached-value", value);
        }

        [Fact]
        public async Task TestCaching_Expiration()
        {
            // Arrange
            var shortCache = new Cache<string>(new CacheConfig { Ttl = TimeSpan.FromMilliseconds(100) });
            shortCache.Set("short-lived", "will-expire");

            // Immediate access (Hit)
            bool isFoundImmediate = shortCache.TryGetValue("short-lived", out var valueImmediate);
            Assert.True(isFoundImmediate);
            Assert.Equal("will-expire", valueImmediate);

            // Wait for expiration
            await Task.Delay(150, TestContext.Current.CancellationToken);

            // After expiration (Miss)
            bool isFoundExpired = shortCache.TryGetValue("short-lived", out var valueExpired);
            Assert.False(isFoundExpired);
            Assert.Null(valueExpired);
        }

        [Fact]
        public async Task TestClientCaching_Integration()
        {
            // Arrange
            int apiCallCount = 0;
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    apiCallCount++;
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent($"{{ 'test': 'data', 'timestamp': {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} }}")
                    };
                });

            var client = new DexPaprikaClient(
                "https://example.com",
                new HttpClient(handlerMock.Object),
                new ClientConfig { Cache = new CacheConfig { Ttl = TimeSpan.FromSeconds(5) } }
            );

            // 1. Duplicate calls
            await client.GetAsync<object>("/test/endpoint");
            await client.GetAsync<object>("/test/endpoint");
            Assert.Equal(1, apiCallCount); // Cached

            // 2. Different endpoints
            await client.GetAsync<object>("/different/endpoint");
            Assert.Equal(2, apiCallCount); // New endpoint causes call

            // 3. Cache clearing
            client.ClearCache();
            await client.GetAsync<object>("/test/endpoint");
            Assert.Equal(3, apiCallCount); // Cleared cache forces call

            // 4. Cache disabling
            client.CacheEnabled = false;
            await client.GetAsync<object>("/test/endpoint");
            await client.GetAsync<object>("/test/endpoint");
            Assert.Equal(5, apiCallCount); // 2 additional bypass hits
        }

        [Fact]
        public void TestMaxCacheSize_Eviction()
        {
            // Arrange
            var smallCache = new Cache<int>(new CacheConfig { MaxSize = 3 });

            // Act
            for (int i = 1; i <= 5; i++)
            {
                smallCache.Set($"key{i}", i);
            }

            // Assert Size Limit
            Assert.Equal(3, smallCache.Count);

            // Assert Eviction Policy (LRU / FIFO oldest gets evicted)
            Assert.False(smallCache.TryGetValue("key1", out _));
            Assert.False(smallCache.TryGetValue("key2", out _));

            Assert.True(smallCache.TryGetValue("key3", out int v3));
            Assert.Equal(3, v3);
            Assert.True(smallCache.TryGetValue("key4", out int v4));
            Assert.Equal(4, v4);
            Assert.True(smallCache.TryGetValue("key5", out int v5));
            Assert.Equal(5, v5);
        }

        #endregion
    }
}
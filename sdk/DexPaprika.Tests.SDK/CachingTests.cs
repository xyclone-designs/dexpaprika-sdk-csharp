using DexPaprika.SDK;
using DexPaprika.SDK.Utils;

using System;
using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public sealed class CachingTests
    {
        private readonly DexPaprikaClient _client;

        public CachingTests()
        {
            _client = new DexPaprikaClient(
                config: new ClientConfig
                {                    
                    Retry = new RetryConfig
                    {
                        MaxRetries = 3,
                        DelaySequenceMs = [500, 1000, 3000]
                    },
                    Cache = new CacheConfig
                    {
                        Ttl = TimeSpan.FromSeconds(30),
                        MaxSize = 10
                    }
                });
        }

        [Fact]
        public async Task NetworksList_CanBeCalledRepeatedly()
        {
            var first = await _client.Networks.ListAsync();
            var second = await _client.Networks.ListAsync();

            Assert.NotEmpty(first);
            Assert.Equal(first.Count, second.Count);
        }

        [Fact]
        public async Task PoolsListByNetwork_CanBeCalledRepeatedly()
        {
            var first = await _client.Pools.ListByNetworkAsync(
                "ethereum",
                new() { Page = 0, Limit = 3 });

            var second = await _client.Pools.ListByNetworkAsync(
                "ethereum",
                new() { Page = 0, Limit = 3 });

            Assert.NotEmpty(first.Pools);
            Assert.Equal(first.Pools.Count, second.Pools.Count);
        }
    }
}



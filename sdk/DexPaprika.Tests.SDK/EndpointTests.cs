using DexPaprika.SDK;
using DexPaprika.SDK.Api;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public sealed class EndpointTests
    {
        private readonly DexPaprikaClient _client = new();

        [Fact]
        public async Task PoolsFilter_Basic()
        {
            var result = await _client.Pools.FilterAsync(
                "ethereum",
                new PoolsApi.FilterOptions
                {
                    Volume24hMin = 100_000,
                    Limit = 5
                });

            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.NotNull(result.PageInfo);

            var pool = result.Results[0];

            Assert.False(string.IsNullOrWhiteSpace(pool.Id));
            Assert.NotNull(pool.Tokens);
            Assert.NotEmpty(pool.Tokens);
        }

        [Fact]
        public async Task PoolsFilter_MultipleParams()
        {
            var result = await _client.Pools.FilterAsync("ethereum", new PoolsApi.FilterOptions
            {
                Volume24hMin = 50_000,
                Txns24hMin = 10,
                SortBy = "volume_24h",
                SortDir = "desc",
                Limit = 3
            });

            Assert.NotNull(result.Results);
        }

        [Fact]
        public async Task TokensGetTop_Basic()
        {
            var result = await _client.Tokens.GetTopAsync("ethereum", new TokensApi.GetTopOptions
            {
                Limit = 5
            });

            Assert.NotNull(result.Tokens);
            Assert.NotEmpty(result.Tokens);
            Assert.NotNull(result.PageInfo);

            var token = result.Tokens[0];

            Assert.False(string.IsNullOrWhiteSpace(token.Address));
            Assert.False(string.IsNullOrWhiteSpace(token.Symbol));
        }

        [Fact]
        public async Task TokensGetTop_WithSort()
        {
            var result = await _client.Tokens.GetTopAsync("ethereum", new TokensApi.GetTopOptions
            {
                OrderBy = "volume_24h",
                Sort = "asc",
                Limit = 3
            });

            Assert.NotNull(result.Tokens);
        }

        [Fact]
        public async Task TokensFilter_Basic()
        {
            var result = await _client.Tokens.FilterAsync("ethereum", new TokensApi.FilterOptions
            {
                Volume24hMin = 100_000,
                Limit = 5
            });

            Assert.NotNull(result.Results);
            Assert.NotEmpty(result.Results);
            Assert.NotNull(result.PageInfo);

            var token = result.Results[0];

            Assert.False(string.IsNullOrWhiteSpace(token.Address));
            Assert.False(string.IsNullOrWhiteSpace(token.Chain));
        }

        [Fact]
        public async Task TokensFilter_WithFdv()
        {
            var result = await _client.Tokens.FilterAsync("ethereum", new TokensApi.FilterOptions
            {
                Volume24hMin = 100_000,
                FdvMin = 1_000_000,
                Limit = 3
            });

            Assert.NotNull(result.Results);
        }

        [Fact]
        public async Task TokensGetMultiPrices_TwoTokens()
        {
            var prices = await _client.Tokens.GetMultiPricesAsync(
                "ethereum",
                [
                    "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2",
                    "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48"
                ]);

            Assert.Equal(2, prices.Count);

            var weth = prices.Single(p => p.Id == "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2");

            Assert.NotNull(weth);
            Assert.NotNull(weth.PriceUsd);
        }

        [Fact]
        public async Task TokensGetMultiPrices_SingleToken()
        {
            var prices = await _client.Tokens.GetMultiPricesAsync("ethereum", [ "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48" ]);

            Assert.Single(prices);
        }

        [Fact]
        public async Task TokensGetMultiPrices_EmptyValidation()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.Tokens.GetMultiPricesAsync( "ethereum", []));

            Assert.Contains("required", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task NetworksList_Works()
        {
            var networks = await _client.Networks.ListAsync();

            Assert.NotEmpty(networks);
        }

        [Fact]
        public async Task PoolsListByNetwork_Works()
        {
            var pools = await _client.Pools.ListByNetworkAsync( "ethereum", new() { Limit = 2 });

            Assert.NotEmpty(pools.Pools);
        }

        [Fact]
        public async Task TokensGetDetails_Works()
        {
            var token = await _client.Tokens.GetDetailsAsync("ethereum", "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48");

            Assert.False(string.IsNullOrWhiteSpace(token.Symbol));
        }

        [Fact]
        public async Task UtilsGetStats_Works()
        {
            var stats = await _client.Utils.GetStatsAsync();

            Assert.True(stats.Chains > 0);
        }
    }
}



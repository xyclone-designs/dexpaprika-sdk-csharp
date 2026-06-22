using DexPaprika.SDK;
using DexPaprika.SDK.Api;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public sealed class BasicTests
    {
        private readonly DexPaprikaClient _client = new();

        [Fact]
        public async Task Networks_List_ShouldContainEthereum()
        {
            var networks = await _client.Networks.ListAsync();

            Assert.NotEmpty(networks);

            var ethereum = networks.FirstOrDefault(n => n.Id == "ethereum");

            Assert.NotNull(ethereum);
        }

        [Fact]
        public async Task Dexes_ListByNetwork_ShouldReturnResults()
        {
            var result = await _client.Dexes.ListByNetworkAsync(
                "ethereum",
                new()
                {
                    Page = 0,
                    Limit = 5
                });

            Assert.NotNull(result);
            Assert.NotEmpty(result.Dexes);
        }

        [Fact]
        public async Task Pools_ListByNetwork_ShouldReturnResults()
        {
            var result = await _client.Pools.ListByNetworkAsync(
                "ethereum",
                new()
                {
                    Page = 0,
                    Limit = 3
                });

            Assert.NotNull(result);
            Assert.NotEmpty(result.Pools);
        }

        [Fact]
        public async Task Pool_GetDetails_ShouldReturnTokens()
        {
            var pools = await _client.Pools.ListByNetworkAsync(
                "ethereum",
                new()
                {
                    Page = 0,
                    Limit = 1
                });

            var pool = Assert.Single(pools.Pools);

            var details = await _client.Pools.GetDetailsAsync(
                pool.Chain,
                pool.Id);

            Assert.NotNull(details);
            Assert.NotEmpty(details.Tokens);
        }

        [Fact]
        public async Task Pool_GetOHLCV_ShouldReturnHistory()
        {
            var pools = await _client.Pools.ListByNetworkAsync(
                "ethereum",
                new()
                {
                    Page = 0,
                    Limit = 1
                });

            var pool = Assert.Single(pools.Pools);

            var date = DateTime.UtcNow
                .AddDays(-7)
                .ToString("yyyy-MM-dd");

            var history = await _client.Pools.GetOhlcvAsync(
                pool.Chain,
                pool.Id,
                date,
                new PoolsApi.GetOhlcvOptions
                {
                    Limit = 3
                });

            Assert.NotNull(history);
            Assert.NotEmpty(history);
        }

        [Fact]
        public async Task Pool_GetTransactions_ShouldReturnResponse()
        {
            var pools = await _client.Pools.ListByNetworkAsync(
                "ethereum",
                new()
                {
                    Page = 0,
                    Limit = 1
                });

            var pool = Assert.Single(pools.Pools);

            var transactions = await _client.Pools.GetTransactionsAsync(
                pool.Chain,
                pool.Id,
                new()
                {
                    Page = 0,
                    Limit = 2
                });

            Assert.NotNull(transactions);
        }

        [Fact]
        public async Task Token_GetDetails_Weth_ShouldReturnToken()
        {
            const string weth =
                "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2";

            var token = await _client.Tokens.GetDetailsAsync(
                "ethereum",
                weth);

            Assert.NotNull(token);
            Assert.Equal("WETH", token.Symbol);
        }

        [Fact]
        public async Task Token_GetPools_Weth_ShouldReturnPools()
        {
            const string weth =
                "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2";

            var pools = await _client.Tokens.GetPoolsAsync(
                "ethereum",
                weth,
                new()
                {
                    Page = 0,
                    Limit = 3
                });

            Assert.NotNull(pools);
            Assert.NotEmpty(pools.Pools);
        }

        [Fact]
        public async Task Search_Bitcoin_ShouldReturnResults()
        {
            var result = await _client.Search.SearchAsync("bitcoin");

            Assert.NotNull(result);

            Assert.True(
                result.Tokens.Count > 0 ||
                result.Pools.Count > 0,
                "Expected at least one search result.");
        }

        [Fact]
        public async Task Utils_GetStats_ShouldReturnStats()
        {
            var stats = await _client.Utils.GetStatsAsync();

            Assert.NotNull(stats);
            Assert.True(stats.Pools > 0);
            Assert.True(stats.Chains > 0);
        }
    }
}



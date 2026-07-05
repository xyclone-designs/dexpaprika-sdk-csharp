using DexPaprika.SDK;
using DexPaprika.SDK.Utils;

using System;
using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public sealed class MigrationTests
    {
        private readonly DexPaprikaClient _client = new();

        [Fact]
        public async Task DeprecatedPoolsList_Throws()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await Assert.ThrowsAsync<DeprecatedEndpointException>(() => _client.Pools.ListAsync());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Theory]
        [InlineData("ethereum")]
        [InlineData("solana")]
        [InlineData("fantom")]
        public async Task ListByNetwork_Works(string network)
        {
            var result = await _client.Pools.ListByNetworkAsync(network, new() { Limit = 3 });

            Assert.NotNull(result);
        }

        [Fact]
        public async Task EmptyNetworkId_Throws()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.Pools.ListByNetworkAsync(string.Empty, new() { Limit = 5 }));

            Assert.Contains("Network ID is required", ex.Message);
        }
    }
}



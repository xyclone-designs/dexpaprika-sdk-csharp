using DexPaprika.SDK;

using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public sealed class CacheControlTests
    {
        [Fact]
        public async Task ClearCache_DoesNotBreakRequests()
        {
            var client = new DexPaprikaClient();

            await client.Networks.ListAsync();

            client.ClearCache();

            var networks = await client.Networks.ListAsync();

            Assert.NotEmpty(networks);
        }

        [Fact]
        public async Task CacheCanBeDisabledAndReEnabled()
        {
            var client = new DexPaprikaClient
            {
                CacheEnabled = false
            };

            var first = await client.Networks.ListAsync();
            var second = await client.Networks.ListAsync();

            Assert.NotEmpty(first);
            Assert.NotEmpty(second);

            client.CacheEnabled = true;

            var third = await client.Networks.ListAsync();
            var fourth = await client.Networks.ListAsync();

            Assert.NotEmpty(third);
            Assert.NotEmpty(fourth);
        }
    }
}



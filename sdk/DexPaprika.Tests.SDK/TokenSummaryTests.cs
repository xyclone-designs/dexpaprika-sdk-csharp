using DexPaprika.SDK;

using System.Threading.Tasks;

namespace DexPaprika.Tests.SDK
{
    public class TokenSummaryTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        [Fact]
        public async Task TestTokenSummary_UpdatesWithSummaryField()
        {
            // Arrange
            var client = new DexPaprikaClient();
            string wethAddress = "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2";

            // Act
            var token = await client.Tokens.GetDetailsAsync("ethereum", wethAddress);

            // Assert
            Assert.NotNull(token);
            _output.WriteLine($"Token: {token.Name} ({token.Symbol})");

            // Verify last_updated
            if (token.LastUpdated != null)
            {
                _output.WriteLine($"Last updated: {token.LastUpdated}");
            }
            else
            {
                _output.WriteLine("last_updated field is not present");
            }

            // Verify summary validation assertions
            Assert.NotNull(token.Summary);
            _output.WriteLine("Summary data:");
            _output.WriteLine($"- Price USD: ${token.Summary.PriceUsd:F2}");
            _output.WriteLine($"- Liquidity USD: ${token.Summary.LiquidityUsd:F2}");
            _output.WriteLine($"- Pools count: {token.Summary.Pools}");

            // Verify time interval metrics
            if (token.Summary.Metrics24h != null)
            {
                _output.WriteLine("24h metrics:");
                _output.WriteLine($"- Volume: ${token.Summary.Metrics24h.VolumeUsd:F2}");

                if (token.Summary.Metrics24h.LastPriceUsdChange.HasValue)
                {
                    _output.WriteLine($"- Price change: {token.Summary.Metrics24h.LastPriceUsdChange.Value:F2}%");
                }
                else
                {
                    _output.WriteLine("- Price change: N/A");
                }
            }
        }
    }
}
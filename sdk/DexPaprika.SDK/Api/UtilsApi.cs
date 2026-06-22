using DexPaprika.SDK.Models;

using System.Threading.Tasks;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// API service for utility endpoints.
    /// </summary>
    public class UtilsApi(DexPaprikaClient client) : BaseApi(client)
    {
        /// <summary>
        /// Gets statistics about the DexPaprika ecosystem.
        /// </summary>
        /// <returns>Statistics about chains, DEXes, pools, and tokens.</returns>
        public Task<Stats> GetStatsAsync(StatsOptions? _ = null)
        {
            return GetAsync<Stats>("/stats");
        }

        public new class Options : BaseApi.Options { }
        public class StatsOptions : Options
        {
            public new static readonly StatsOptions _Default = new();
            public new static class Defaults { }
        }
    }
}

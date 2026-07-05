using DexPaprika.SDK.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DexPaprika.SDK.Api
{
    /// <summary>
    /// API service for network-related endpoints.
    /// </summary>
    public class NetworksApi(DexPaprikaClient client) : BaseApi(client)
    {
        /// <summary>
        /// Gets a list of supported blockchain networks.
        /// </summary>
        /// <returns>List of supported networks.</returns>
        public Task<List<Network>> ListAsync(ListOptions? _ = null)
        {
            return GetAsync<List<Network>>("/networks");
        }

        public new class Options : BaseApi.Options { }
        public class ListOptions : Options
        {
            public new static readonly ListOptions _Default = new();
            public new static class Defaults { }
            public new static class Params { }
        }
    }
}

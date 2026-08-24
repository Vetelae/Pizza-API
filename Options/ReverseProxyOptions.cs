using System.Net;

namespace Pizza_API.Options
{
    public class ReverseProxyOptions
    {
        public const string SectionName = "ReverseProxy";

        public bool Enabled { get; set; }
        public int ForwardLimit { get; set; } = 1;
        public string[] KnownProxies { get; set; } = [];
        public string[] KnownNetworks { get; set; } = [];

        public bool IsValid()
        {
            if (!Enabled)
                return true;

            if (KnownProxies == null || KnownNetworks == null)
                return false;

            return ForwardLimit > 0
                && KnownProxies.Length + KnownNetworks.Length > 0
                && KnownProxies.All(value => IPAddress.TryParse(value, out _))
                && KnownNetworks.All(value => IPNetwork.TryParse(value, out _));
        }
    }
}

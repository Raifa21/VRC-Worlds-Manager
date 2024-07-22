using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;

namespace VRC_Favourite_Manager.Services
{
    internal class VRChatAPIService
    {
        private HttpClient _Client;

        private CookieContainer _cookieContainer;
        public class Configuration
        {
            public CookieContainer CookieContainer { get; set; }
            public IWebProxy Proxy { get; set; }
            public string UserAgent { get; set; }
            public string BasePath { get; set; }
            public ConcurrentDictionary<string, string> DefaultHeaders { get; set; }
            public ConcurrentDictionary<string, string> ApiKey { get; set; }
            public ConcurrentDictionary<string, string> ApiKeyPrefix { get; set; }
            public List<IReadOnlyDictionary<string, object>> Servers { get; set; }
            public Dictionary<string, List<IReadOnlyDictionary<string, object>>> OperationServers { get; set; }
            public int Timeout { get; set; }

            public Configuration()
            {
                Proxy = null;
                UserAgent = "vrchatapi-csharp";
                BasePath = "https://vrchat.com/api/1";
                DefaultHeaders = new ConcurrentDictionary<string, string>();
                ApiKey = new ConcurrentDictionary<string, string>();
                ApiKeyPrefix = new ConcurrentDictionary<string, string>();
                Servers = new List<IReadOnlyDictionary<string, object>>()
                {
                    new Dictionary<string, object>
                    {
                        {"url", "https://vrchat.com/api/1"},
                        {"description", "No description provided"},
                    }
                };
                OperationServers = new Dictionary<string, List<IReadOnlyDictionary<string, object>>>();
                CookieContainer = new CookieContainer(); // Initialize CookieContainer
                Timeout = 100000;
            }
        }

        public VRChatAPIService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true,

            };
            _Client = new HttpClient(handler);
            // Set initial configuration values
            _Client.BaseAddress = new Uri("https://vrchat.com/api/1");
            _Client.Timeout = TimeSpan.FromSeconds(10000);
            _Client.DefaultRequestHeaders.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
        }


        public async Task<bool> VerifyAuthTokenAsync(string authToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/config?auth={authToken}");
            request.Headers.Add("Accept", "application/json");
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return true;
        }

        public async 
    }

}

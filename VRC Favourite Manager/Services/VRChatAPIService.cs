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


        private HttpClient _client;
        private CookieContainer _cookieContainer;
        public VRChatAPIService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer
            };
            _client = new HttpClient(handler);
        }




        public async Task<bool> VerifyAuthTokenAsync(string authToken)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10000);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/config?auth={authToken}");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return true;
        }
    }

}

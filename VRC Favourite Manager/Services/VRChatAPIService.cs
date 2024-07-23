using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Collections.Concurrent;
using System.Text.Encodings.Web;
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
                        { "url", "https://vrchat.com/api/1" },
                        { "description", "No description provided" },
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
            var request = new HttpRequestMessage(HttpMethod.Get, "/config?auth={authToken}");
            request.Headers.Add("Accept", "application/json");
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return true;
        }

        public async Task<bool> VerifyLoginAsync(string username, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/auth/user?");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", CreateAuthString(username,password));
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Task<string> readTask = response.Content.ReadAsStringAsync();
            if 

        }

        /// <summary>
        /// Generates a auth string according to VRC's API requirements.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The encoded authorization string.</returns>
        private static string CreateAuthString(string username, string password)
        {
            var encodedUsername = WebUtility.UrlEncode(username);
            var encodedPassword = WebUtility.UrlEncode(password);
            var authString = $"{encodedUsername}:{encodedPassword}";
            var base64AuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            Debug.WriteLine($"Basic {base64AuthString}");
            return $"Basic {base64AuthString}";
        }

        /// <summary>
        /// Checks if the user requires Email 2FA to login.
        /// </summary>
        /// <returns>Returns if the user requires Email 2FA or not.</returns>
        private static bool requiresEmail2FA(

    }
}

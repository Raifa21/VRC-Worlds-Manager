using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading;
using VRC_Favourite_Manager.Common;
using System.Text.Json;

namespace VRC_Favourite_Manager.Services
{
    internal class VRChatAPIService
    {
        private HttpClient _Client;

        private CookieContainer _cookieContainer;

        private string _authToken;

        private string _twoFactorAuthToken;

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

        /// <summary>
        /// Verifies if the user has provided the correct auth token.
        /// <param name="authToken">The authentication token.</param>
        /// <param name="twoFactorAuthToken">The two factor authentication token.</param>
        /// <returns>Returns if the auth token is valid or not.</returns>
        public async Task<bool> VerifyAuthTokenAsync(string authToken, string twoFactorAuthToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/auth");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Cookie", $"auth={authToken};twoFactorAuth={twoFactorAuthToken}");
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Check if verification was successful.
            var responseString = await response.Content.ReadAsStringAsync();

            // Deserialize the response string to a JSON object.
            var authResponse = JsonSerializer.Deserialize<Models.VerifyAuthTokenResponse>(responseString);
            if (authResponse.ok)
            {
                _authToken = authToken;
                _twoFactorAuthToken = twoFactorAuthToken;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Verifies if the user has provided the correct login credentials.
        /// If valid, the auth token is stored in the response header. This is passed onto StoreAuthToken to be stored.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Returns if the user has successfully logged in or not.</returns>
        /// <exception cref="VRCRequiresTwoFactorAuthException">User requires 2FA, contains a TwoFactorAuthType of either "email" or "default".</exception>
        public async Task<bool> VerifyLoginAsync(string username, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/auth/user?");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", CreateAuthString(username,password));
            var response = await _Client.SendAsync(request);
            // This throws a HttpRequestException if the status code is not a success code.
            response.EnsureSuccessStatusCode();

            // Pass the header to store the auth token.
            StoreAuthToken(response.Headers);

            // Check if the user requires a certain 2FA method. 
            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString.Contains("emailOtp"))
            {
                throw new VRCRequiresTwoFactorAuthException("email");
            }
            else if (responseString.Contains("totp"))
            {
                throw new VRCRequiresTwoFactorAuthException("default");
            }
            else
            {
                return true;
            }
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


        public async Task<bool> Authenticate2FA(string twoFactorCode, string twoFactorAuthType)
        {
            HttpRequestMessage request; 
            if (twoFactorAuthType == "email")
            {
                request = new HttpRequestMessage(HttpMethod.Post, "/auth/twofactorauth/emailotp/verify");
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, "/auth/twofactorauth/totp/verify");
            }
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
            var content = new StringContent($"{{\\n  \\\"code\\\": \\\"{twoFactorCode}\\\"\\n}}", null, "application/json");
            request.Content = content;
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Pass the header to store the auth token.
            StoreAuthToken(response.Headers);

            return true;
        }


        /// <summary>
        /// Retrieves and stores the auth token or the two factor auth token from the response header.
        /// </summary>
        /// <param name="headers">The response header obtained from /auth/user </param>
        private void StoreAuthToken(HttpResponseHeaders headers)
        {
            if(headers.TryGetValues("set-cookie", out var authValues))
            {
                var authToken = authValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(authToken))
                {
                    if(authToken.Contains("twoFactorAuth"))
                    {
                        var twoFactorAuthToken = authToken.Split(';')[1];
                        twoFactorAuthToken = twoFactorAuthToken.Replace("twoFactorAuth=", "");
                        _twoFactorAuthToken = twoFactorAuthToken;
                    }
                    else if (authToken.Contains("auth"))
                    {
                        authToken = authToken.Split(';')[0];
                        authToken = authToken.Replace("auth=", "");
                        _authToken = authToken;
                    }
                }
            }
        }

    }
}

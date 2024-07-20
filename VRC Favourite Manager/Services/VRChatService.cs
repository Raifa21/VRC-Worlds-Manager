using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using VRC_Favourite_Manager.Models;
using System.Diagnostics;
using System.Net;
using Windows.Media.Protection.PlayReady;
using VRC_Favourite_Manager.Common;
using System.Linq;

namespace VRC_Favourite_Manager.Services
{
    public class VRChatService
    {
        private readonly Configuration _config;
        private readonly ApiClient client;
        private AuthenticationApi authApi;
        private UsersApi userApi;
        private WorldsApi worldsApi;
        private ApiResponse<CurrentUser> response;
        private bool gotApi = false;
        public bool RequiresEmailotp { get; private set; }

        public VRChatService()
        {
            System.Diagnostics.Debug.WriteLine("Creating VRChatService");
            _config = new Configuration
            {
                BasePath = "https://vrchat.com/api/1",
                UserAgent = "VRC Favourite Manager/dev 0.0.1 Raifa"
            };
            client = new ApiClient();
        }

        public void SetAPIKey(string apiKey)
        {
            _config.AddApiKey("auth", apiKey);
        }

        public ApiResponse<VerifyAuthTokenResult> CheckAuthentication()
        {
            authApi = new AuthenticationApi(client, client, _config);
            return authApi.VerifyAuthTokenWithHttpInfo();
        }

        public bool Login(string username, string password)
        {
            _config.Username = username;
            _config.Password = password;

            authApi = new AuthenticationApi(client, client, _config);

            try
            {
                response = authApi.GetCurrentUserWithHttpInfo();
                if (response.Headers.TryGetValue("Set-Cookie", out var cookies))
                {
                    var authCookieHeader = cookies.FirstOrDefault();
                    if (authCookieHeader != null)
                    {
                        var authCookie = authCookieHeader.Split(';')[0];
                        authCookie = authCookie.Replace("authcookie=", ""); // Ensure this matches the actual cookie name
                        System.Diagnostics.Debug.WriteLine("Current auth token:");
                        System.Diagnostics.Debug.WriteLine(authCookie);
                        _config.AddApiKey("auth", authCookie);
                        this.gotApi = true;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Cannot obtain cookie: not logged in yet!");
                }
                if (RequiresEmail2FA(response))
                {
                    RequiresEmailotp = true;
                    return true;
                }
                else
                {
                    RequiresEmailotp = false;
                    return true;
                }
            }
            catch (ApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Login failed: " + e.Message);
                throw new VRCIncorrectCredentialsException();
            }
        }

        private static bool RequiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            return resp.RawContent.Contains("emailOtp");
        }

        public Verify2FAEmailCodeResult VerifyEmail2FA(string twoFactorAuthCode)
        {
            try
            {
                var apiResponse = authApi.Verify2FAEmailCodeWithHttpInfo(new TwoFactorEmailCode(twoFactorAuthCode));
                if (apiResponse.Headers.TryGetValue("Set-Cookie", out var cookies))
                {
                    var authCookieHeader = cookies.FirstOrDefault();
                    if (authCookieHeader != null)
                    {
                        var authCookie = authCookieHeader.Split(';')[0];
                        authCookie = authCookie.Replace("authcookie=", ""); // Ensure this matches the actual cookie name
                        System.Diagnostics.Debug.WriteLine("Current auth token:");
                        System.Diagnostics.Debug.WriteLine(authCookie);
                        _config.AddApiKey("auth", authCookie);
                        return apiResponse.Data;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to obtain cookie");
                }
            }
            catch (ApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Incorrect OTP code: " + e.Message);
                throw new VRCIncorrectCredentialsException();
            }
            throw new VRCIncorrectCredentialsException();
        }

        public Verify2FAResult Verify2FA(string twoFactorAuthCode)
        {
            try
            {
                var apiResponse = authApi.Verify2FAWithHttpInfo(new TwoFactorAuthCode(twoFactorAuthCode));
                if (apiResponse.Headers.TryGetValue("Set-Cookie", out var cookies))
                {
                    var authCookieHeader = cookies.FirstOrDefault();
                    if (authCookieHeader != null)
                    {
                        var authCookie = authCookieHeader.Split(';')[0];
                        authCookie = authCookie.Replace("authcookie=", ""); // Ensure this matches the actual cookie name
                        System.Diagnostics.Debug.WriteLine("Current auth token:");
                        System.Diagnostics.Debug.WriteLine(authCookie);
                        _config.AddApiKey("auth", authCookie);
                        return apiResponse.Data;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Cannot obtain cookie");
                }
            }
            catch (ApiException e)
            {
                System.Diagnostics.Debug.WriteLine("2FA Verification failed: " + e.Message);
                throw new VRCIncorrectCredentialsException();
            }
            throw new VRCIncorrectCredentialsException();
        }

    public void StoreAuth()
        {
            var configManager = new ConfigManager();
            if (_config.ApiKey.TryGetValue("auth", out string apiKey))
            {
                configManager.WriteConfig("auth = \"" + apiKey + "\"");
                System.Diagnostics.Debug.WriteLine("auth = \"" + apiKey + "\"");
                System.Diagnostics.Debug.WriteLine("API key written to config file.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error writing API key to config file.");
            }
            if (_config.ApiKey.TryGetValue("twoFactorAuth", out string twoFactorAuthKey))
            {
                configManager.WriteConfig("twoFactorAuth = \"" + twoFactorAuthKey + "\"");
                System.Diagnostics.Debug.WriteLine("twoFactorAuth = \"" + twoFactorAuthKey + "\"");
                System.Diagnostics.Debug.WriteLine("2FA key written to config file.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error writing 2FA key to config file.");
            }
        }



        /// <summary>
        /// Called initially when the user does not have any saved worlds. Looks through all favourited worlds and returns them.
        /// 
        /// </summary>
        /// <returns>A list of worlds, each containing info about itself</returns>
        /// <exception cref="VRCNotLoggedInException">When the user is not logged in</exception>
        /// <exception cref="VRCAPIException">When the API does not return data as expected.</exception>
        public async Task<List<Models.WorldModel>> GetAllFavoriteWorldsAsync()
        {
            try
            {
                int pageSize = 100;
                int currentPage = 0;
                var apiInstance = new WorldsApi(client, client, _config);
                bool hasNext = true;
                List<LimitedWorld> worlds = new List<LimitedWorld>();
                while (hasNext)
                {
                    var tempFavoriteList =
                        await apiInstance.GetFavoritedWorldsAsync(null, null, pageSize, null, pageSize * currentPage);
                    worlds.AddRange(tempFavoriteList);
                    if (tempFavoriteList.Count < pageSize)
                    {
                        hasNext = false;
                    }
                    currentPage++;
                }

                List<Models.WorldModel> favoriteWorlds = new List<Models.WorldModel>();
                foreach (var world in worlds)
                {
                    favoriteWorlds.Add(new WorldModel
                    {
                        ImageUrl = world.ImageUrl,
                        Name = world.Name,
                        AuthorName = world.AuthorName,
                        RecommendedCapacity = world.RecommendedCapacity,
                        Capacity = world.Capacity
                    });

                }
                return favoriteWorlds;
            }
            catch (ApiException ex) when (ex.ErrorCode == 401)
            {
                throw new VRCNotLoggedInException();
            }
            catch (ApiException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception when calling API: {0}", ex.Message);
                throw new VRCAPIException();
            }
        }
        /// <summary>
        ///  Returns the 60 most recently favourited worlds. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="VRCNotLoggedInException">When the user is not logged in</exception>
        /// <exception cref="VRCAPIException">When the API does not return data as expected.</exception>
        public async Task<List<Models.WorldModel>> GetFavoriteWorldsAsync()
        {
            try
            {
                var apiInstance = new WorldsApi(_config);
                var worlds = await apiInstance.GetFavoritedWorldsAsync();
                List<Models.WorldModel> favoriteWorlds = new List<Models.WorldModel>();
                foreach (var world in worlds)
                {
                    favoriteWorlds.Add(new WorldModel
                    {
                        ImageUrl = world.ImageUrl,
                        Name = world.Name,
                        AuthorName = world.AuthorName,
                        RecommendedCapacity = world.RecommendedCapacity,
                        Capacity = world.Capacity
                    });

                }
                return favoriteWorlds;
            }
            catch (ApiException ex) when (ex.ErrorCode == 401)
            {
                throw new VRCNotLoggedInException();
            }
            catch (ApiException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception when calling API: {0}", ex.Message);
                throw new VRCAPIException();
            }
        }
    }
}

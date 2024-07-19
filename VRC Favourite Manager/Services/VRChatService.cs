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

namespace VRC_Favourite_Manager.Services
{
    public class VRChatService
    {
        //is not readonly as config can be re-assigned
        private readonly Configuration _config;
        private readonly ApiClient client;
        private AuthenticationApi authApi;
        private UsersApi userApi;
        private WorldsApi worldsApi;
        private ApiResponse<CurrentUser> response;
        public bool RequiresEmailotp { get; private set; }


        public VRChatService()
        {
            System.Diagnostics.Debug.WriteLine("Creating VRChatService");
            _config = new Configuration();
            _config.BasePath = "https://vrchat.com/api/1";
            _config.UserAgent = "VRC Favourite Manager/dev 0.0.1 Raifa";

            client = new ApiClient();
        }


        public void SetAPIKey(string apiKey)
        {
            _config.AddApiKey("auth" , apiKey);
        }

        public ApiResponse<VerifyAuthTokenResult> CheckAuthentication()
        {
            authApi = new AuthenticationApi(client, client, _config);
            return authApi.VerifyAuthTokenWithHttpInfo();
        }
        
        /// <summary>
        /// Sets the login and password for the user. 
        /// </summary>
        public bool Login(string username, string password)
        {
            _config.Username = username;
            _config.Password = password;

            authApi = new AuthenticationApi(client, client, _config);

            try
            {
                response = authApi.GetCurrentUserWithHttpInfo();
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
                throw new VRCIncorrectCredentialsException();
            }
        }


        private static bool RequiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            if (resp.RawContent.Contains("emailOtp"))
            {
                return true;
            }

            return false;
        }

        public Verify2FAEmailCodeResult VerifyEmail2FA(string twoFactorAuthCode)
        {
            try
            {
                return authApi.Verify2FAEmailCode(new TwoFactorEmailCode(twoFactorAuthCode));
            }
            catch (ApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Incorrect OTP code.");
                throw new VRCIncorrectCredentialsException();
            }
        }
        public Verify2FAResult Verify2FA(string twoFactorAuthCode)
        {
            try
            {
                return authApi.Verify2FA(new TwoFactorAuthCode(twoFactorAuthCode));
            }
            catch (ApiException e)
            {
                throw new VRCIncorrectCredentialsException();
            }
        }

        public bool ConfirmLogin()
        {
            response = authApi.GetCurrentUserWithHttpInfo();
            System.Diagnostics.Debug.WriteLine("Status Code: " + response.StatusCode);
            System.Diagnostics.Debug.WriteLine("Logged in as {0}", response.Data.DisplayName);

            if (response.Headers.TryGetValue("Set-Cookie", out var cookies))
            {
                _config.ApiKey["auth"] = cookies[0];
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No cookies found.");
                return false;
            }
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

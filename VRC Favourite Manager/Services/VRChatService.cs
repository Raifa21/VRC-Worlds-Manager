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
            _config = new Configuration();
            _config.UserAgent = "VRC Favourite Manager/0.0.1 Raifa";

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
            // We can just use a super simple string.Contains() check
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

        public bool confirmLogin()
        {
            response = authApi.GetCurrentUserWithHttpInfo();
            return response.StatusCode == HttpStatusCode.Accepted;
        }
        public void StoreAuth()
        {
            var configManager = new ConfigManager();
            try
            {
                configManager.WriteConfig("auth = \"" + _config.ApiKey + "\"");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing API key to config file: " + e.Message);
            }
        }



        /// <summary>
        /// Called initially when the user does not have any saved worlds. Looks through all favourited worlds and returns them.
        /// 
        /// </summary>
        /// <returns>A list of worlds, each containing info about itself</returns>
        /// <exception cref="VRCNotLoggedInException">When the user is not logged in</exception>
        public async Task<List<Models.WorldModel>> InitialGetFavoriteWorldsAsync()
        {
            try
            {
                var favoriteModels = await InitialGetFavoriteListAsync();
                var favoriteWorlds = new List<Models.WorldModel>();

                foreach (var favorite in favoriteModels)
                {
                    try
                    {
                        var world = await worldsApi.GetWorldAsync(favorite.FavoriteId);
                        favoriteWorlds.Add(new WorldModel
                        {
                            ImageUrl = world.ImageUrl,
                            Name = world.Name,
                            RecommendedCapacity = world.RecommendedCapacity,
                            Capacity = world.Capacity
                        });
                    }
                    catch (ApiException ex) when (ex.ErrorCode == 401)
                    {
                        throw new VRCNotLoggedInException();
                    }
                    catch (ApiException ex)
                    {
                        Console.WriteLine("Exception when calling API: {0}", ex.Message);
                        throw;
                    }
                }

                return favoriteWorlds;
            }
        }


        private async Task<List<Favorite>> InitialGetFavoriteListAsync()
        {
            //do pagination, max is 400 worlds
            int pageSize = 100;
            int currentPage = 0;
            var apiInstance = new FavoritesApi(client, client, _config);
            var favoriteModels = new List<Favorite>();
            try
            {
                bool hasNext = true;
                while (hasNext)
                {
                    var favorites = await apiInstance.GetFavoritesAsync(pageSize, currentPage * pageSize, "world");
                    favoriteModels.AddRange(favorites);
                    if (favorites.Count < pageSize)
                    {
                        hasNext = false;
                    }
                    currentPage++;

                }
            }

            catch (ApiException ex) {
                Console.WriteLine("Exception when calling API: {0}", ex.Message);
                throw new VRCAPIException();
            }
            return favoriteModels;
        }
    }
}

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

namespace VRC_Favourite_Manager.Services
{
    public class VRChatService
    {
        private readonly Configuration _config;
        private readonly ApiClient client;
        private readonly AuthenticationApi authApi;
        private readonly UsersApi userApi;
        private readonly WorldsApi worldApi;

        public VRChatService()
        {
            // Create a client to hold all our cookies :D
            client = new ApiClient();

            // Create an instance of the auth api so we can verify and log in
            authApi = new AuthenticationApi(_config);
        }
        public bool CheckAuthentication()
        {

            try
            {
                ApiResponse<CurrentUser> response = authApi.GetCurrentUserWithHttpInfo();
                return response.StatusCode == HttpStatusCode.OK;

            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling AuthenticationApi.GetCurrentUser: " + e.Message);
                return false;
            }
        }

        public async Task<List<WorldModel>> GetFavoriteWorldsAsync(List<FavouriteModel> favoriteModels)
        {
            var favoriteWorlds = new List<WorldModel>();

            foreach (var favorite in favoriteModels)
            {
                if (favorite.type == "world")
                {
                    try
                    {
                        var world = await worldApi.GetWorldAsync(favorite.favouriteId);
                        favoriteWorlds.Add(new WorldModel
                        {
                            imageUrl = world.ImageUrl,
                            name = world.Name,
                            recommendedCapacity = world.RecommendedCapacity.ToString(),
                            capacity = world.Capacity.ToString()
                        });
                    }
                    catch (ApiException ex)
                    {
                        Console.WriteLine("Exception when calling API: {0}", ex.Message);
                    }
                }
            }

            return favoriteWorlds;
        }

        public async Task<bool> LoginAwait(string username, string password)
        {
            Configuration config = new Configuration();
            config.Username = username;
            config.Password = password;

            config.UserAgent = "VRC Favourite Manager/0.0.1 Raifa";

            AuthenticationApi authApi = new AuthenticationApi(client, client, config);

            try
            {
                ApiResponse<CurrentUser> currentUserResp = authApi.GetCurrentUserWithHttpInfo();
                if (requiresEmail2FA(currentUserResp))
                {
                    authApi.Verify2FAEmailCode(new TwoFactorEmailCode("123456"));
                }
                else
                {
                    authApi.Verify2FA(new TwoFactorAuthCode("123456"));
                }
                return true;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Exception when calling API: {ex.Message}");
                return false;
            }
        }

        // Function that determines if the api expects email2FA from an ApiResponse
        private static bool requiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            // We can just use a super simple string.Contains() check
            return resp.RawContent.Contains("emailOtp");
        }
    }
}

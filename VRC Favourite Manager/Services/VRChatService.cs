using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using VRC_Favourite_Manager.Models;
using System.Diagnostics;

namespace VRC_Favourite_Manager.Services
{
    public class VRChatService
    {
        private readonly Configuration _config;
        private readonly AuthenticationApi _authApi;
        private readonly ApiClient client;
        private readonly AuthenticationApi authApi;
        private readonly UsersApi userApi;
        private readonly WorldsApi worldApi;

        public VRChatService()
        {
            // Create a configuration for us to log in
            _config = new Configuration();
            _config.BasePath = "https://vrchat.com/api/1";
            _config.AddApiKey("auth", "YOUR_API_KEY");
            // Create a client to hold all our cookies :D
            client = new ApiClient();

            // Create an instance of the auth api so we can verify and log in
            _authApi = new AuthenticationApi(_config);
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                VerifyAuthTokenResult result = await _authApi.VerifyAuthTokenAsync();

                Debug.WriteLine("VerifyAuthToken: " + result);
                return true;
            }
            catch (ApiException ex)
            {
                Debug.WriteLine("Exception when calling API: " + ex.Message);
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

        // Function that determines if the api expects email2FA from an ApiResponse
        private static bool requiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            // We can just use a super simple string.Contains() check
            return resp.RawContent.Contains("emailOtp");
        }
    }
}

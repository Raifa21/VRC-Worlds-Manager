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
        private readonly Configuration _config;
        private readonly ApiClient client;
        private readonly AuthenticationApi authApi;
        private readonly UsersApi userApi;
        private readonly WorldsApi worldsApi;

        public VRChatService(string username, string password)
        {
            // Create a client to hold all our cookies :D
            client = new ApiClient();
            _config = new Configuration();
            authApi = new AuthenticationApi(client,client,_config);
            userApi = new UsersApi(client,client,_config);
            worldsApi = new WorldsApi(client,client,_config);

        }


        
        /// <summary>
        /// Checks if the user is logged in using the VRChat API, this returns Http status code 200 if logged in with user info, and 401 if not logged in
        /// </summary>
        /// <returns>if logged in or not</returns>
        public bool CheckAuthentication()
        {
            _config.UserAgent = "VRC Favourite Manager/0.0.1 Raifa";

            authApi = 
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

        /// <summary>
        /// Get's the user's favourite worlds. Up to 100 worlds can be returned.
        /// 
        /// </summary>
        /// <param name="favoriteModels"></param>
        /// <returns></returns>
        /// <exception cref="VRCNotLoggedInException">When the user is not logged in,</exception>
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
                    catch (ApiException ex) when (ex.ErrorCode==401)
                    {
                        throw new VRCNotLoggedInException();
                    }
                    catch(ApiException ex)
                    {
                        Console.WriteLine("Exception when calling API: {0}", ex.Message);
                        throw;
                    }

                }
            }

            return favoriteWorlds;
        }
    }
}

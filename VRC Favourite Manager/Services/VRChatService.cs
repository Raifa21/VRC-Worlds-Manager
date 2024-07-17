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


        public VRChatService()
        {
            _config = new Configuration();
            _config.UserAgent = "VRC Favourite Manager/0.0.1 Raifa";

            client = new ApiClient();
        }



        
        /// <summary>
        /// Checks if the user is logged in using the VRChat API, this returns Http status code 200 if logged in with user info, and 401 if not logged in
        /// </summary>
        /// <returns>if logged in or not</returns>
        public void CheckAuthentication(string username, string password)
        {

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new VRCIncorrectCredentialsException();
            }
            _config.Username = username;
            _config.Password = password;
            authApi = new AuthenticationApi(client, client, _config);
            userApi = new UsersApi(client, client, _config);
            worldsApi = new WorldsApi(client, client, _config);

            try
            {
                ApiResponse<CurrentUser> response = authApi.GetCurrentUserWithHttpInfo();
                if (requiresEmail2FA(response)) // If the API wants us to send an Email OTP code
                {
                    throw 
                }
                else
                {
                    // requiresEmail2FA returned false, so we use secret-based 2fa verification
                    // authApi.VerifyRecoveryCode(new TwoFactorAuthCode("12345678")); // To Use a Recovery Code
                    authApi.Verify2FA(new TwoFactorAuthCode("123456"));
                }

            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling AuthenticationApi.GetCurrentUser: " + e.Message);
            }
        }
        static bool requiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            // We can just use a super simple string.Contains() check
            if (resp.RawContent.Contains("emailOtp"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the user's favourite worlds. Up to 100 worlds can be returned.
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
                try
                {
                    var world = await worldsApi.GetWorldAsync(favorite.favouriteId);
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

            return favoriteWorlds;
        }
    }
}

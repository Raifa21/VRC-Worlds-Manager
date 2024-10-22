using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Globalization;
using System.Net.Http.Headers;
using VRC_Favourite_Manager.Common;
using System.Text.Json;
using Serilog;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Services
{
    public interface IVRChatAPIService
    {
        Task<bool> VerifyAuthTokenAsync(string authToken, string twoFactorAuthToken);
        Task<bool> VerifyLoginWithAuthTokenAsync(string authToken, string twoFactorAuthToken);
        Task<bool> VerifyLoginAsync(string username, string password);
        Task<bool> Authenticate2FAAsync(string twoFactorCode, string twoFactorAuthType);
        Task<bool> LogoutAsync();
        Task<List<Models.WorldModel>> GetFavoriteWorldsAsync(int n, int offset);
        Task CreateInstanceAsync(string worldId, string instanceType, string region);

        Task CreateGroupInstanceAsync(string worldId, string groupId, string region, string instanceType,
            List<string> roleIds,
            bool queueEnabled);

        Task<List<GetUserGroupsResponse>> GetGroupsAsync();
        Task<List<GroupRolesModel>> GetGroupRolesAsync(string groupId);
        Task<List<string>> GetUserRoleAsync(string groupId);


    }
    public class VRChatAPIService : IVRChatAPIService
    {
        private HttpClient _Client;

        private CookieContainer _cookieContainer;

        private string _authToken;

        private string _twoFactorAuthToken;

        private string _userId;

        public VRChatAPIService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true,

            };
            _Client = new HttpClient(handler);
        }

        /// <summary>
        /// Verifies if the user has provided the correct auth token.
        /// <param name="authToken">The authentication token.</param>
        /// <param name="twoFactorAuthToken">The two factor authentication token.</param>
        /// <returns>Returns if the auth token is valid or not.</returns>
        public async Task<bool> VerifyAuthTokenAsync(string authToken, string twoFactorAuthToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth");

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={authToken};twoFactorAuth={twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize the response string to a JSON object.
                var authResponse = JsonSerializer.Deserialize<Models.VerifyAuthTokenResponse>(responseString);
                Log.Information(authResponse.ok.ToString());
                if (authResponse.ok)
                {
                    _authToken = authToken;
                    _twoFactorAuthToken = twoFactorAuthToken;
                    GetUserId();
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Information("Error: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks if the user has successfully logged in with the auth token.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="twoFactorAuthToken"></param>
        /// <returns>If the user has successfully logged in</returns>
        /// <exception cref="VRCIncorrectCredentialsException"></exception>
        public async Task<bool> VerifyLoginWithAuthTokenAsync(string authToken, string twoFactorAuthToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth/user?");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={authToken};twoFactorAuth={twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                GetUserId();
                return true;
            }
            catch (Exception e)
            {
                Log.Information("Error: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the user's ID from the API.
        /// </summary>
        private async Task<string> GetUserId()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth/user?");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                // Parse the response and check if the 'id' property exists
                var jsonDocument = JsonDocument.Parse(responseString);
                if (jsonDocument.RootElement.TryGetProperty("id", out JsonElement idElement))
                {
                    _userId = idElement.GetString();
                    return _userId;
                }

                return null;
            }
            catch (Exception e)
            {
                Log.Information("Error: " + e.Message);
                return null;
            }
        }


        /// <summary>
        /// Verifies if the user has provided the correct login credentials.
        /// If valid, the auth token is stored in the response header. This is passed onto StoreAuthToken to be stored.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>If the user has successfully logged in or not</returns>
        /// <exception cref="VRCRequiresTwoFactorAuthException">User requires 2FA, contains a TwoFactorAuthType of either "email" or "default".</exception>
        public async Task<bool> VerifyLoginAsync(string username, string password)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth/user?");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Authorization", CreateAuthString(username, password));
                Log.Information(request.RequestUri.ToString());
                if (!string.IsNullOrEmpty(_twoFactorAuthToken))
                {
                    if (!string.IsNullOrEmpty(_authToken))
                    {
                        request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                    }
                    else
                    {
                        request.Headers.Add("Cookie", $"twoFactorAuth={_twoFactorAuthToken}");
                    }
                }

                var response = await _Client.SendAsync(request);


                Log.Information($"Status Code: {response.StatusCode}");

                // Output the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                Log.Information("Response Content:");
                Log.Information(responseContent);


                if (!response.IsSuccessStatusCode)
                {
                    // Handle failed status code based on the status code
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            Log.Information("Incorrect credentials.");
                            Log.Information("Error: " + responseContent);
                            Log.Information("Status Code: " + response.StatusCode);
                            Log.Information("Reason: " + response.ReasonPhrase);
                            throw new VRCIncorrectCredentialsException();
                        default:
                            Log.Information("Service Unavailable.");
                            Log.Information("Error: " + responseContent);
                            Log.Information("Status Code: " + response.StatusCode);
                            Log.Information("Reason: " + response.ReasonPhrase);
                            throw new VRCServiceUnavailableException(responseContent);
                    }
                }

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
                    JsonDocument.Parse(responseString).RootElement.TryGetProperty("id", out JsonElement id);
                    _userId = id.GetString();
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                Log.Information("Error: " + e.Message);
                throw new VRCServiceUnavailableException(e.Message);
            }
            catch (VRCRequiresTwoFactorAuthException e)
            {
                throw;
            }
            catch (VRCIncorrectCredentialsException e)
            {
                throw;
            }
            catch (VRCServiceUnavailableException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Log.Information("Error: " + e.Message);
                throw new VRCServiceUnavailableException(e.Message);
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
            return $"Basic {base64AuthString}";
        }

        /// <summary>
        /// Authenticates the user with the 2FA code provided.
        /// </summary>
        /// <param name="twoFactorCode"></param>
        /// <param name="twoFactorAuthType"></param>
        /// <returns>boolean which represents if the provided 2FA code is valid.</returns>
        public async Task<bool> Authenticate2FAAsync(string twoFactorCode, string twoFactorAuthType)
        {
            try
            {
                HttpRequestMessage request;
                if (twoFactorAuthType == "email")
                {
                    request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/auth/twofactorauth/emailotp/verify");
                }
                else
                {
                    request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/auth/twofactorauth/totp/verify");
                }
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken}");
                var content = new StringContent($"{{\n  \"code\": \"{twoFactorCode}\"\n}}", Encoding.UTF8, "application/json");

                request.Content = content;
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();


                // Pass the header to store the auth token.
                StoreAuthToken(response.Headers);


                GetUserId();

                return true;
            }
            catch (HttpRequestException e)
            {
                Log.Information("Error: " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                Log.Information("Error: " + e.Message);
                throw new VRCServiceUnavailableException(e.Message);
            }
        }

        /// <summary>
        /// Retrieves and stores the auth token or the two factor auth token from the response header.
        /// </summary>
        /// <param name="headers">The response header obtained from /auth/user </param>
        private void StoreAuthToken(HttpResponseHeaders headers)
        {
            var configManager = new ConfigManager();
            if (headers.TryGetValues("set-cookie", out var authValues))
            {
                var authToken = authValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(authToken))
                {
                    if (authToken.Contains("twoFactorAuth"))
                    {
                        var twoFactorAuthToken = authToken.Split(';')[0];
                        twoFactorAuthToken = twoFactorAuthToken.Replace("twoFactorAuth=", "");
                        _twoFactorAuthToken = twoFactorAuthToken;
                        configManager.WriteToConfig("twoFactorAuth", twoFactorAuthToken);
                    }
                    else if (authToken.Contains("auth"))
                    {
                        authToken = authToken.Split(';')[0];
                        authToken = authToken.Replace("auth=", "");
                        _authToken = authToken;
                        configManager.WriteToConfig("auth", authToken);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the authentication cookie.
        /// </summary>
        /// <returns>If the user has successfully logged out or not.</returns>
        public async Task<bool> LogoutAsync()
        {
            try
            {
                ConfigManager configManager = new ConfigManager();
                configManager.Logout();


                var request = new HttpRequestMessage(HttpMethod.Put, "https://vrchat.com/api/1/logout");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize the response string to a JSON object.
                var authResponse = JsonSerializer.Deserialize<Models.LogoutResponse>(responseString);
                Log.Information(authResponse.message);

                return authResponse.message == "Ok!";
            }
            catch (HttpRequestException e)
            {
                Log.Information("Error: " + e.Message);
                return false;
            }
            catch (JsonException e)
            {
                Log.Information("Error: " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                Log.Information("Error: " + e.Message);
                return false;
            }
           
        }

        /// <summary>
        /// Obtains the user's favorite worlds. 
        /// </summary>
        /// <param name="n">The number of worlds to return. This application will default to 100.</param>
        /// <param name="offset">Offset to allow pagination of worlds.</param>
        /// <returns>A list of worlds, sorted by date added to favorites. </returns>
        public async Task<List<Models.WorldModel>> GetFavoriteWorldsAsync(int n, int offset)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://vrchat.com/api/1/worlds/favorites?n={n}&offset={offset}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                Log.Information(request.RequestUri.ToString());
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                Log.Information(responseString);

                var responseWorlds =
                    JsonSerializer.Deserialize<List<Models.ListFavoriteWorldsResponse>>(responseString);
                var worldModels = new List<Models.WorldModel>();
                foreach (var world in responseWorlds)
                {
                    var worldModel = new Models.WorldModel
                    {
                        ThumbnailImageUrl = world.ThumbnailImageUrl,
                        WorldName = world.Name,
                        WorldId = world.Id,
                        AuthorName = world.AuthorName,
                        AuthorId = world.AuthorId,
                        Capacity = world.Capacity,
                        LastUpdate = world.UpdatedAt.ToString(CultureInfo.InvariantCulture)?[..10],
                        Description = world.Description,
                        Visits = world.Visits,
                        Favorites = world.Favorites,
                    };
                    worldModels.Add(worldModel);
                }

                return worldModels;
            }
            catch (JsonException ex)
            {
                Log.Information($"Deserialization error: {ex.Message}");
                return null;
            }
            catch (HttpRequestException ex)
            {
                Log.Information($"Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Log.Information($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ListFavoriteWorldsResponse> GetWorldByIdAsync(string worldId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://vrchat.com/api/1/worlds/{worldId}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var responseWorld = JsonSerializer.Deserialize<ListFavoriteWorldsResponse>(responseString);
                return responseWorld;
            }
            catch (JsonException ex)
            {
                Log.Information($"Deserialization error: {ex.Message}");
                return null;
            }
            catch (HttpRequestException ex)
            {
                Log.Information($"Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Log.Information($"Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates an instance of a world, and creates an invite for the user.
        /// </summary>
        /// <param name="worldId">An ID which represents the world to be created.</param>
        /// <param name="instanceType">The instance type of the instance being created. Allowed parameters are: "public","friends+","friends","invite+","invite".</param>
        /// <param name="region">The region of the instance being created. Allowed parameters are: "usw","use","jp","eu" </param>
        /// <returns>Returns the instanceId of the instance which was created.</returns>
        /// <exception cref="VRCNotLoggedInException">When the authentication tokens fails to authenticate the user.</exception>
        public async Task CreateInstanceAsync(string worldId, string instanceType, string region)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/instances");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");

                Debug.WriteLine(instanceType);

                string type;
                string ownerIdNullable;
                string canRequestInvite;
                switch (instanceType)
                {
                    case "Public":
                        type = "public";
                        canRequestInvite = "false";
                        ownerIdNullable = null;
                        break;
                    case "Friends+":
                        type = "hidden";
                        canRequestInvite = "false";
                        ownerIdNullable = _userId;
                        break;
                    case "Friends":
                        type = "friends";
                        canRequestInvite = "false";
                        ownerIdNullable = _userId;
                        break;
                    case "Invite+":
                        type = "private";
                        canRequestInvite = "true";
                        ownerIdNullable = _userId;
                        break;
                    default:
                        type = "private";
                        canRequestInvite = "false";
                        ownerIdNullable = _userId;
                        break;
                }

                region = region.ToLower();
                //usw is represented as "us" in api
                if (region == "usw")
                {
                    region = "us";
                }

                Log.Information($"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"{type}\",\n  \"region\": \"{region}\",\n  \"ownerId\": \"{ownerIdNullable}\",\n  \"queueEnabled\": false,\n  \"canRequestInvite\": {canRequestInvite}\n }}");

                if(ownerIdNullable == null)
                {
                    var content = new StringContent(
                        $"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"{type}\",\n  \"region\": \"{region}\",\n  \"queueEnabled\": false,\n  \"canRequestInvite\": {canRequestInvite}\n }}",
                        null, "application/json");
                    request.Content = content;
                }
                else
                {
                    var content = new StringContent(
                        $"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"{type}\",\n  \"region\": \"{region}\",\n  \"ownerId\": \"{ownerIdNullable}\",\n  \"queueEnabled\": false,\n  \"canRequestInvite\": {canRequestInvite}\n }}",
                        null, "application/json");
                    request.Content = content;
                }


                var response = await _Client.SendAsync(request);

                response.EnsureSuccessStatusCode();


                var responseString = await response.Content.ReadAsStringAsync();
                JsonDocument.Parse(responseString).RootElement.TryGetProperty("instanceId", out JsonElement id);
                var instanceId = id.GetString();

                InviteSelfAsync(worldId, instanceId);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Error: " + e.Message);
                Log.Information("Error: " + e.Message);
                throw new VRCNotLoggedInException();
            }
        }

        /// <summary>
        /// Creates an instance of a world, and creates an invite for the user.
        /// Created instance is a group instance
        /// </summary>
        /// <param name="worldId">An ID which represents the world to be created.</param>
        /// <param name="groupId">An ID which represents the group </param>
        /// <param name="region">The region of the instance being created. Allowed parameters are: "usw","use","jp","eu" </param>
        /// <param name="instanceType">The instance type of the instance being created. Allowed parameters are: "public","friends+","friends","invite+","invite".</param>
        /// <param name="roleIds">The roles which have access to the instance. If no value is passed, all roles are allowed. </param>
        /// <param name="queueEnabled">Boolean which represents if queues are allowed or not when instances are full.</param>
        /// <returns>Task to call API to create instance, and invite the user.</returns>
        /// <exception cref="VRCNotLoggedInException"></exception>
        public async Task CreateGroupInstanceAsync(string worldId, string groupId, string region, string instanceType, List<string> roleIds,
            bool queueEnabled)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/instances");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");

                region = region.ToLower();
                //usw is represented as "us" in api
                if (region == "usw")
                {
                    region = "us";
                }
                var queueEnabled_String = queueEnabled ? "true" : "false";

                string groupAccessType;
                switch (instanceType)
                {
                    case "Group":
                        groupAccessType = "members";
                        break;
                    case "Group+":
                        groupAccessType = "plus";
                        break;
                    default:
                        groupAccessType = "public";
                        break;
                }

                if (roleIds == null || roleIds.Count == 0)
                {
                    Log.Information($"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"group\",\n  \"region\": \"{region}\",\n  \"ownerId\": \"{groupId}\",\n  \"queueEnabled\": {queueEnabled_String},\n  \"groupAccessType\": \"{groupAccessType}\",\n  \"canRequestInvite\": false,\n  \"inviteOnly\": false\n}}");
                    var content = new StringContent(
                        $"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"group\",\n  \"region\": \"{region}\",\n  \"ownerId\": \"{groupId}\",\n  \"queueEnabled\": {queueEnabled_String},\n  \"groupAccessType\": \"{groupAccessType}\",\n  \"canRequestInvite\": false,\n  \"inviteOnly\": false\n}}",
                        null, "application/json");
                    request.Content = content;
                }
                else
                {
                    var roleIds_formatted = "[\n    ";
                    foreach (var roleId in roleIds)
                    {
                        if (roleIds.IndexOf(roleId) != roleIds.Count - 1)
                        {
                            roleIds_formatted += $"\"{roleId}\",\n";
                        }
                        else
                        {
                            roleIds_formatted += $"\"{roleId}\"";
                        }
                    }
                    roleIds_formatted += "\n]";


                    Log.Information(
                        $"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"group\",\n  \"region\": \"{region}\",\n  \"ownerId\": \"{groupId}\",\n  \"roleIds\": {roleIds_formatted},\n  \"queueEnabled\": {queueEnabled_String},\n  \"groupAccessType\": \"{groupAccessType}\",\n  \"canRequestInvite\": false,\n  \"inviteOnly\": false\n}}");
                    var content = new StringContent(
                        $"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"group\",\n  \"region\": \"{region}\",\n  \"ownerId\": \"{groupId}\",\n  \"roleIds\": {roleIds_formatted},\n  \"queueEnabled\": {queueEnabled_String},\n  \"groupAccessType\": \"{groupAccessType}\",\n  \"canRequestInvite\": false,\n  \"inviteOnly\": false\n}}",
                        null, "application/json");
                    request.Content = content;
                }


                
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                JsonDocument.Parse(responseString).RootElement.TryGetProperty("instanceId", out JsonElement id);
                var instanceId = id.GetString();

                InviteSelfAsync(worldId, instanceId);
            }
            catch (HttpRequestException e)
            {
                Log.Information("Error: " + e.Message);
                throw new VRCNotLoggedInException();
            }
        }

        /// <summary>
        /// Gets the groups the user is in.
        /// </summary>
        /// <returns>A list of user's groups</returns>
        /// <exception cref="VRCNotLoggedInException"></exception>
        public async Task<List<GetUserGroupsResponse>> GetGroupsAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://vrchat.com/api/1/users/{_userId}/groups");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJsonList = JsonSerializer.Deserialize<List<GetUserGroupsResponse>>(responseString);
                return responseJsonList;
            }
            catch (HttpRequestException)
            {
                throw new VRCNotLoggedInException();
            }
        }

        /// <summary>
        /// Gets the roles in the group
        /// </summary>
        /// <param name="groupId">The ID which represents the group</param>
        /// <returns>A list of group's roles</returns>
        /// <exception cref="VRCNotLoggedInException"></exception>
        public async Task<List<GroupRolesModel>> GetGroupRolesAsync(string groupId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://vrchat.com/api/1/groups/{groupId}/roles");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var responseJsonList = JsonSerializer.Deserialize<List<GetGroupRolesResponse>>(responseString);
                var roleIds = new List<GroupRolesModel>();
                foreach (var role in responseJsonList)
                {
                    roleIds.Add(new GroupRolesModel
                    {
                        Name = role.Name,
                        Id = role.Id,
                        Permissions = role.Permissions,
                        IsManagementRole = role.IsManagementRole,
                        Order = role.Order
                    });
                }
                return roleIds;
            }
            catch (HttpRequestException e)
            {
                Log.Information("Error: " + e.Message);
                throw new VRCNotLoggedInException();
            }
        }

        /// <summary>
        /// Gets the user's roles in the group
        /// </summary>
        /// <param name="groupId">The ID which represents the group</param>
        /// <returns>A list of role id's which represent the user's group roles</returns>
        /// <exception cref="VRCNotLoggedInException"></exception>
        public async Task<List<string>> GetUserRoleAsync(string groupId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://vrchat.com/api/1/groups/{groupId}/members/{_userId}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<GetGroupMemberResponse>(responseString);
                return responseJson.RoleIds;
            }
            catch (HttpRequestException)
            {
                throw new VRCNotLoggedInException();
            }
        }



        /// <summary>
        /// Invites the user to the instance provided.
        /// </summary>
        /// <param name="worldId"></param>
        /// <param name="instanceId"></param>
        /// <exception cref="VRCFailedToCreateInviteException">When the invite has failed to be created.</exception>
        private async void InviteSelfAsync(string worldId, string instanceId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://vrchat.com/api/1/invite/myself/to/{worldId}:{instanceId}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Worlds Manager/v1.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");



                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                throw new VRCNotLoggedInException();
            }
        }
    }
}
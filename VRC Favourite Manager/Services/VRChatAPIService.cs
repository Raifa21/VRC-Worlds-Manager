using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace VRC_Favourite_Manager.Services
{
    internal class VRChatAPIService
    {
        public VRChatAPIService()
        {
        }

        public async Task<bool> VerifyAuthTokenAsync(string authToken)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10000);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("auth", authToken);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return true;
        }
    }

}

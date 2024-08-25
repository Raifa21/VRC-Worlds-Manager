using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Services
{
    public class GitHubSupporterService
    {
        private readonly HttpClient _httpClient;

        public GitHubSupporterService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<SupportersData> GetSupportersAsync(string repoOwner, string repoName, string filePath)
        {
            var url = $"https://raw.githubusercontent.com/{repoOwner}/{repoName}/master/{filePath}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SupportersData>(content);
        }
    }

    public class Supporter
    {
        public string Name { get; set; }
        public int Tier { get; set; }
        public string Color { get; set; }
    }

    public class TiersWithoutSupporters
    {
        public TierDetails Tier1 { get; set; }
        public TierDetails Tier5 { get; set; }
    }

    public class TierDetails
    {
        public int Tier { get; set; }
        public string Color { get; set; }
        public string Note { get; set; }
    }

    public class SupportersData
    {
        public List<Supporter> Supporters { get; set; }
        public TiersWithoutSupporters TiersWithoutSupporters { get; set; }
    }
}
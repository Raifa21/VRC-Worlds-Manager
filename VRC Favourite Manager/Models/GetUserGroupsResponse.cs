using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRC_Favourite_Manager.Models
{
    public class GetUserGroupsResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("shortCode")] public string ShortCode { get; set; }

        [JsonPropertyName("discriminator")] public string Discriminator { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("iconId")] public string? IconId { get; set; }

        [JsonPropertyName("iconUrl")] public string? IconUrl { get; set; }

        [JsonPropertyName("bannerId")] public string? BannerId { get; set; }

        [JsonPropertyName("bannerUrl")] public string? BannerUrl { get; set; }

        [JsonPropertyName("privacy")] public string? Privacy { get; set; }

        [JsonPropertyName("lastPostCreatedAt")]
        public DateTime? LastPostCreatedAt { get; set; }

        [JsonPropertyName("ownerId")] public string OwnerId { get; set; }

        [JsonPropertyName("memberCount")] public int MemberCount { get; set; }

        [JsonPropertyName("groupId")] public string GroupId { get; set; }

        [JsonPropertyName("memberVisibility")] public string MemberVisibility { get; set; }

        [JsonPropertyName("isRepresenting")] public bool IsRepresenting { get; set; }

        [JsonPropertyName("mutualGroup")] public bool MutualGroup { get; set; }

        [JsonPropertyName("lastPostReadAt")] public DateTime? LastPostReadAt { get; set; }
    }

}
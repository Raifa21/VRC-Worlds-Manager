using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRC_Favourite_Manager.Models
{
    public class GetGroupMemberResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("groupId")] public string GroupId { get; set; }

        [JsonPropertyName("userId")] public string UserId { get; set; }

        [JsonPropertyName("isRepresenting")] public bool IsRepresenting { get; set; }

        [JsonPropertyName("roleIds")] public List<string> RoleIds { get; set; }

        [JsonPropertyName("mRoleIds")] public List<string> MRoleIds { get; set; }

        [JsonPropertyName("joinedAt")] public DateTime? JoinedAt { get; set; }

        [JsonPropertyName("membershipStatus")] public string MembershipStatus { get; set; }

        [JsonPropertyName("visibility")] public string Visibility { get; set; }

        [JsonPropertyName("isSubscribedToAnnouncements")] public bool IsSubscribedToAnnouncements { get; set; }

        [JsonPropertyName("createdAt")] public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("bannedAt")] public DateTime? BannedAt { get; set; }

        [JsonPropertyName("managerNotes")] public string? ManagerNotes { get; set; }

        [JsonPropertyName("lastPostReadAt")] public DateTime? LastPostReadAt { get; set; }

        [JsonPropertyName("hasJoinedFromPurchase")] public bool HasJoinedFromPurchase { get; set; }
    }

}
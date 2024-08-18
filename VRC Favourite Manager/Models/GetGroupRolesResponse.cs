using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRC_Favourite_Manager.Models
{
    public class GetGroupRolesResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("groupId")] public string GroupId { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("description")] public string Description { get; set; }
        
        [JsonPropertyName("isSelfAssignable")] public bool IsSelfAssignable { get; set; }

        [JsonPropertyName("permissions")] public List<string> Permissions { get; set; }

        [JsonPropertyName("isManagementRole")] public bool IsManagementRole { get; set; }

        [JsonPropertyName("requiresTwoFactor")] public bool RequiresTwoFactor { get; set; }
        
        [JsonPropertyName("requiresPurchase")] public bool RequiresPurchase { get; set; }

        [JsonPropertyName("order")] public int Order { get; set; }

        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")] public DateTime UpdatedAt { get; set; }
        
    }

}
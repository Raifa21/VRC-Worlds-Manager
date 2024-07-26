using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRC_Favourite_Manager.Models
{
    public class ListFavoriteWorldsResponse
    {
        [JsonPropertyName("authorId")]
        public string AuthorId { get; set; }

        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("recommendedCapacity")]
        public int? RecommendedCapacity { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("favorites")]
        public int Favorites { get; set; }

        [JsonPropertyName("visits")]
        public int? Visits { get; set; }

        [JsonPropertyName("heat")]
        public int Heat { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("labsPublicationDate")]
        public string LabsPublicationDate { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("occupants")]
        public int Occupants { get; set; }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }

        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }

        [JsonPropertyName("previewYoutubeId")]
        public string? PreviewYoutubeId { get; set; }

        [JsonPropertyName("publicationDate")]
        public string PublicationDate { get; set; }

        [JsonPropertyName("releaseStatus")]
        public string ReleaseStatus { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonPropertyName("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonPropertyName("unityPackages")]
        public List<UnityPackage> UnityPackages { get; set; } = new List<UnityPackage>();

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("udonProducts")]
        public List<string>? UdonProducts { get; set; } = new List<string>();
    }

    public class UnityPackage
    {
        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("unityVersion")]
        public string UnityVersion { get; set; }
    }
}

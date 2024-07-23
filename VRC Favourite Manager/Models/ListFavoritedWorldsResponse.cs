using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VRC_Favourite_Manager.Models
{
    public class ListFavoriteWorldsResponse
    {
        
        public string AuthorId { get; set; }

        
        public string AuthorName { get; set; }

        
        public int Capacity { get; set; }

        
        public int RecommendedCapacity { get; set; }

        
        public DateTime CreatedAt { get; set; }

        
        public int Favorites { get; set; }

        
        public int Visits { get; set; }

        
        public int Heat { get; set; }

        
        public string Id { get; set; }

        
        public string ImageUrl { get; set; }

        public string LabsPublicationDate { get; set; }

        
        public string Name { get; set; }

        
        public int Occupants { get; set; }

        public string Organization { get; set; }

        
        public int Popularity { get; set; }

        public string PreviewYoutubeId { get; set; }

        public string PublicationDate { get; set; }

        
        public string ReleaseStatus { get; set; }

        
        public List<string> Tags { get; set; }

        
        public string ThumbnailImageUrl { get; set; }

        
        public List<UnityPackage> UnityPackages { get; set; }

        
        public DateTime UpdatedAt { get; set; }

        
        public List<string> UdonProducts { get; set; }
    }

    public class UnityPackage
    {
        
        public string Platform { get; set; }

        
        public string UnityVersion { get; set; }
    }
}

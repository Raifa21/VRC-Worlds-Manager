using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VRC_Favourite_Manager.Models
{
    public class CreateInstanceResponse
    {
        public bool Active { get; set; }
        public bool CanRequestInvite { get; set; }
        public int Capacity { get; set; }
        public bool Full { get; set; }
        public string Id { get; set; }
        public string InstanceId { get; set; }
        public string Location { get; set; }
        public int NUsers { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public bool Permanent { get; set; }
        public string PhotonRegion { get; set; }
        public Platforms Platforms { get; set; }
        public string Region { get; set; }
        public string SecureName { get; set; }
        public string ShortName { get; set; }
        public List<string> Tags { get; set; }
        public string Type { get; set; }
        public string WorldId { get; set; }
        public string Hidden { get; set; }
        public string Friends { get; set; }
        public string Private { get; set; }
        public bool QueueEnabled { get; set; }
        public int QueueSize { get; set; }
        public int RecommendedCapacity { get; set; }
        public bool RoleRestricted { get; set; }
        public bool Strict { get; set; }
        public int UserCount { get; set; }
        public World World { get; set; }
        public List<User> Users { get; set; }
        public string GroupAccessType { get; set; }
        public bool HasCapacityForYou { get; set; }
        public string Nonce { get; set; }
        public DateTime ClosedAt { get; set; }
        public bool HardClose { get; set; }
    }

    public class Platforms
    {
        public int Android { get; set; }
        public int StandaloneWindows { get; set; }
    }

    public class World
    {
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int Capacity { get; set; }
        public int RecommendedCapacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public int Favorites { get; set; }
        public bool Featured { get; set; }
        public int Heat { get; set; }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public List<List<object>> Instances { get; set; }
        public string LabsPublicationDate { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public int Occupants { get; set; }
        public string Organization { get; set; }
        public int Popularity { get; set; }
        public string PreviewYoutubeId { get; set; }
        public int PrivateOccupants { get; set; }
        public int PublicOccupants { get; set; }
        public string PublicationDate { get; set; }
        public string ReleaseStatus { get; set; }
        public List<string> Tags { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public List<UnityPackage> UnityPackages { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; }
        public int Visits { get; set; }
        public List<string> UdonProducts { get; set; }
    }

    public class User
    {
        public string Bio { get; set; }
        public List<string> BioLinks { get; set; }
        public string CurrentAvatarImageUrl { get; set; }
        public string CurrentAvatarThumbnailImageUrl { get; set; }
        public List<string> CurrentAvatarTags { get; set; }
        public string DeveloperType { get; set; }
        public string DisplayName { get; set; }
        public string FallbackAvatar { get; set; }
        public string Id { get; set; }
        public bool IsFriend { get; set; }
        public string LastPlatform { get; set; }
        public string ProfilePicOverride { get; set; }
        public string Pronouns { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public List<string> Tags { get; set; }
        public string UserIcon { get; set; }
        public string Location { get; set; }
        public string FriendKey { get; set; }
    }
}

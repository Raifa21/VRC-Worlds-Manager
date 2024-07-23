using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Models
{
    public class WorldModel
    {
        public string ThumbnailImageUrl { get; set; }
        public string WorldName { get; set; }
        public string WorldId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public int RecommendedCapacity { get; set; }
        public int Capacity { get; set; }
        public DateTime LastUpdate { get; set; }
    }

}

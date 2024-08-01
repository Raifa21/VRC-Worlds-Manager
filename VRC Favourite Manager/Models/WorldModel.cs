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
        public int Capacity { get; set; }
        public string LastUpdate { get; set; }
        public string Description { get; set; }
        public int? Visits { get; set; }
        public int Favorites { get; set; }
        public List<string> Tags { get; set; }
        
    }

}

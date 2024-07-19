using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Models
{
    public class WorldModel
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string AuthorName { get; set; }
        public int RecommendedCapacity { get; set; }
        public int Capacity { get; set; }
    }

}

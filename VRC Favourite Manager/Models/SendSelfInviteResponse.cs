using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Models
{
    public class SendSelfInviteResponse
    {
        public string Message { get; set; }
        public int Status_code { get; set; }
    }
}
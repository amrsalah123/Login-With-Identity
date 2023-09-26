using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTask.Models
{
    public class ApplicationUser:IdentityUser
    {

        [ForeignKey("user")]

        public string MentorId { get; set; }
        public string ImageUrl { get; set; }
        public string job { get; set; }
        public ApplicationUser user { get; set; }
    }
}

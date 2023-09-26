using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTask.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Your Job")]
        public string Job { get; set; }
        [Compare("Password",ErrorMessage ="Password and Confirmation password are not match")]
        [Required]
        [DataType(DataType.Password)]

        public string Confirm { get; set; }
        [NotMapped]
        public IFormFile file  { get; set; }

        public string MentorId { get; set; }
    }
}

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web.App.Models.Lead
{
    public class ContactModel
    {
        public int? UserId { get; set; }

        [Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email"), Required, EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Message"), Required, MaxLength(500)]
        public string Message { get; set; }

        public string ClientIp { get; set; }

        public bool IsSuccess { get; set; }

    }
}

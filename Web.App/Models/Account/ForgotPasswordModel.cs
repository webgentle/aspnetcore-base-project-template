using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Web.App.Models.Account
{
    public class ForgotPasswordModel
    {
        [Display(Name = "Registered email address"), Required, EmailAddress]
        public string Email { get; set; }
        public bool IsSuccess { get; set; }
    }
}

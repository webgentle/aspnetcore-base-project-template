using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.App.Models.Account
{
    public class ConfirmEmailModel
    {
        public string Email { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsConfirmed { get; set; }
    }
}

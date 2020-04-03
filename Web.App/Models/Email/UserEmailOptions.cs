using System.Collections.Generic;
using System.Net.Mail;

namespace Web.App.Models.Email
{
    public class UserEmailOptions
    {
        public List<string> ToEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public LinkedResource Resource { get; set; }
        public string ServerPath { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
        public List<KeyValuePair<string, string>> PlaceHolders { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
    }
}

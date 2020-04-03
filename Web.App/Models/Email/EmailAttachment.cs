using System.IO;

namespace Web.App.Models.Email
{
    public class EmailAttachment
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string MediaType { get; set; }
    }
}

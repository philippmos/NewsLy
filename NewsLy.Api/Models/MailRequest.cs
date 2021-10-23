using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace NewsLy.Api.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string RequestIp { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
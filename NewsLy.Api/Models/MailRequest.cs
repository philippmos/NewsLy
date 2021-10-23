using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsLy.Api.Models
{
    public class MailRequest : BaseEntity
    {
        public string ToEmail { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string RequestIp { get; set; }

        [NotMapped]
        public List<IFormFile> Attachments { get; set; }
    }
}
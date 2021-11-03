using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewsLy.Api.Dtos.Mailing
{
    public class MailingCreateDto
    {
        public string ToEmail { get; set; }
        public int? ToMailingListId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public List<IFormFile> Attachments { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using NewsLy.Api.Models;

namespace NewsLy.Api.Dtos.Recipient
{
    public class RecipientCreateDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Required]
        public string Email { get; set; }
        public Gender Gender { get; set; }

        [Required]
        public int MailingListId { get; set; }
    }
}
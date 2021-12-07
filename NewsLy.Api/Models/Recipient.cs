using System;
using NewsLy.Api.Enums;

namespace NewsLy.Api.Models
{
    public class Recipient : BaseEntity
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }

        public DateTime? ConfirmationMailSentDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public bool IsVerified { get; set; }
    }
}
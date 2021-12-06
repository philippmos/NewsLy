using System;

namespace NewsLy.Api.Models
{
    public class Recipient : BaseEntity
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }

        public DateTime ConfirmationMailSentDate { get; set; }
        public DateTime UserConfirmationDate { get; set; }
    }

    public enum Gender 
    {
        Male,
        Female,
        NotSpecified
    }
}
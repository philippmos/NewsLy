using System.Collections.Generic;

namespace NewsLy.Api.Models
{
    public class MailingList : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Recipient> Recipients { get; set; }
    }
}
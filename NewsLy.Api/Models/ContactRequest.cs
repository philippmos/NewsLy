namespace NewsLy.Api.Models
{
    [Dapper.Contrib.Extensions.Table("ContactRequests")]
    public class ContactRequest : MailRequest
    {
        public string Message { get; set; }
    }
}
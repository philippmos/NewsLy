namespace NewsLy.Api.Dtos.Mailing
{
    public class MailRequestDto
    {        
        public string ToEmail { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
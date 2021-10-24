namespace NewsLy.Api.Models
{
    public class TrackingUrl : BaseEntity
    {
        public string TrackingId { get; set; }
        public string TargetUrl { get; set; }
        public int AccessCount { get; set; }
        public bool IsActive { get; set; }
    }
}
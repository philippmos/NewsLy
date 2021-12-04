using System.ComponentModel.DataAnnotations;

namespace NewsLy.Api.Dtos.Tracking
{
    public class TrackingUrlCreateDto
    {
        [Required]
        public string TargetUrl { get; set; }
    }
}
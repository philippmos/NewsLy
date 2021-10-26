using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly ILogger<TrackingController> _logger;
        private readonly ITrackingUrlRepository _trackingUrlRepository;

        public TrackingController(
            ILogger<TrackingController> logger,
            ITrackingUrlRepository trackingUrlRepository
        )
        {
            _logger = logger;
            _trackingUrlRepository = trackingUrlRepository;
        }

        [HttpGet]
        public void Get([FromQuery] string t)
        {
            if(string.IsNullOrEmpty(t))
            {
                return;
            }

            var trackingUrl = _trackingUrlRepository.FindByTrackingId(t);

            if(trackingUrl == null)
            {
                return;
            }

            Response.Redirect(trackingUrl.TargetUrl);

            return;
        }
    }
}

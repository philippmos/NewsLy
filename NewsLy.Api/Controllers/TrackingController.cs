using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AutoMapper;

using NewsLy.Api.Dtos.Tracking;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services.Interfaces;

namespace NewsLy.Api.Controllers
{
    public class TrackingController : ApiBaseController
    {
        private readonly ILogger<TrackingController> _logger;
        private readonly IMapper _mapper;
        private readonly ITrackingService _trackingService;

        public TrackingController(
            ILogger<TrackingController> logger,
            IMapper mapper,
            ITrackingService trackingService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _trackingService = trackingService;
        }

        [HttpGet]
        public IEnumerable<TrackingUrlDto> GetAll()
        {
            return _trackingService.GetAllActive();
        }

        [HttpGet("rdr")]
        [AllowAnonymous]
        public void RedirectUrl([FromQuery] string t)
        {
            var targetUrl = _trackingService.GetTargetUrlAndIncreaseAccessCount(t);

            if (!string.IsNullOrEmpty(targetUrl))
            {
                Response.Redirect(targetUrl);
            }
            
            return;
        }

        [HttpPost]
        public IActionResult Create([FromForm]TrackingUrlCreateDto trackingUrlCreateDto)
        {
            if (_trackingService.CreateNewTrackingUrl(trackingUrlCreateDto) == null)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}

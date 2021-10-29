using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Dtos.Tracking;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services.Interfaces;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly ILogger<TrackingController> _logger;
        private readonly IMapper _mapper;
        private readonly ITrackingUrlRepository _trackingUrlRepository;
        private readonly ITrackingService _trackingService;

        public TrackingController(
            ILogger<TrackingController> logger,
            IMapper mapper,
            ITrackingUrlRepository trackingUrlRepository,
            ITrackingService trackingService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _trackingUrlRepository = trackingUrlRepository;
            _trackingService = trackingService;
        }

        [HttpGet]
        public List<TrackingUrlDto> GetAll()
        {
            return _mapper.Map<List<TrackingUrlDto>>(
                _trackingUrlRepository.GetAllActive()
            );
        }


        [HttpGet("redirection")]
        public void Redirection([FromQuery] string t)
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

            trackingUrl.AccessCount++;

            _trackingUrlRepository.Update(trackingUrl);

            Response.Redirect(trackingUrl.TargetUrl);

            return;
        }

        [HttpPost("create")]
        public IActionResult Create([FromForm]TrackingUrlCreateDto trackingUrlCreateDto)
        {
            var trackingUrl = _mapper.Map<TrackingUrl>(trackingUrlCreateDto);
            trackingUrl.TrackingId = _trackingService.GenerateTrackingId();
            trackingUrl.IsActive = true;

            var createdTrackingUrl = _trackingUrlRepository.Add(trackingUrl);

            if (createdTrackingUrl.Id == 0)
            {
                return BadRequest();
            }

            return Ok(createdTrackingUrl);
        }
    }
}

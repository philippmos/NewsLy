using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Dtos.Tracking;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly ILogger<TrackingController> _logger;
        private readonly IMapper _mapper;
        private readonly ITrackingUrlRepository _trackingUrlRepository;

        public TrackingController(
            ILogger<TrackingController> logger,
            IMapper mapper,
            ITrackingUrlRepository trackingUrlRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _trackingUrlRepository = trackingUrlRepository;
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

            Response.Redirect(trackingUrl.TargetUrl);

            return;
        }
    }
}

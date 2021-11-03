using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services.Interfaces;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailingsController : ControllerBase
    {
        private readonly ILogger<MailingsController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMailingService _mailingService;
        private readonly IContactRequestRepository _contactRequestRepository;

        public MailingsController(
            ILogger<MailingsController> logger,
            IMapper mapper,
            IConfiguration configuration,
            IMailingService mailingService,
            IContactRequestRepository contactRequestRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _mailingService = mailingService;
            _contactRequestRepository = contactRequestRepository;
        }

        [HttpGet]
        public List<ContactRequest> Get()
        {
            return _contactRequestRepository.GetAll();
        }

        [HttpPost]
        public async Task<IActionResult> SendMailAsync([FromForm] MailingCreateDto mailRequestDto)
        {
            if (string.IsNullOrEmpty(mailRequestDto.ToEmail) && !mailRequestDto.ToMailingListId.HasValue)
            {
                return BadRequest("ToEmail or ToMailingListId Parameter is required.");
            }

            try
            {
                ContactRequest contactRequest = _mapper.Map<ContactRequest>(mailRequestDto);

                await _mailingService.SendMailingAsync(contactRequest, mailRequestDto.TrackLinks);
                
                _contactRequestRepository.Add(contactRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return StatusCode(500);
        }

        [HttpGet("testvalue")]
        public IActionResult TestValue()
        {
            return Ok($"TestValue: { _configuration["TestValue"] }");
        }

        [HttpGet("lists")]
        public IEnumerable<MailingListDto> GetAllMailingLists()
        {
            return _mailingService.GetAllMailingLists();
        }
    }
}

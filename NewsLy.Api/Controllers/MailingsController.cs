using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using AutoMapper;

using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services.Interfaces;
using NewsLy.Api.Dtos.Recipient;
using NewsLy.Api.Enums;
using Microsoft.AspNetCore.Authorization;

namespace NewsLy.Api.Controllers
{
    public class MailingsController : ApiBaseController
    {
        private readonly ILogger<MailingsController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMailingService _mailingService;
        private readonly IMailRequestRepository _mailRequestRepository;

        public MailingsController(
            ILogger<MailingsController> logger,
            IMapper mapper,
            IConfiguration configuration,
            IMailingService mailingService,
            IMailRequestRepository mailRequestRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _mailingService = mailingService;
            _mailRequestRepository = mailRequestRepository;
        }

        [HttpGet]
        public IEnumerable<MailRequestDto> Get()
        {
            return _mapper.Map<List<MailRequestDto>>(_mailRequestRepository.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> SendMailAsync([FromForm] MailingCreateDto mailCreateDto)
        {
            if (string.IsNullOrEmpty(mailCreateDto.ToEmail) && !mailCreateDto.ToMailingListId.HasValue)
            {
                return BadRequest("ToEmail or ToMailingListId Parameter is required.");
            }

            try
            {
                var mailRequest = await _mailingService.SendMailingAsync(mailCreateDto, MailType.ContactRequest);

                if (mailRequest == null)
                {
                    return BadRequest();
                }
                
                _mailRequestRepository.Add(mailRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return StatusCode(500);
        }

        [HttpGet("lists")]
        public IEnumerable<MailingListDto> GetAllMailingLists()
        {
            return _mailingService.GetAllMailingLists();
        }

        [HttpPost("recipient")]
        public async Task<IActionResult> AddRecipientToMailingList([FromForm] RecipientCreateDto recipientCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return await _mailingService.CreateRecipientForMailingList(recipientCreateDto) ? Ok() : BadRequest();
        }

        [HttpGet("recipient-verification")]
        [AllowAnonymous]
        public IActionResult RecipientEmailVerification([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }


            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using AutoMapper;

using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services.Interfaces;
using NewsLy.Api.Dtos.Recipient;

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
        public async Task<IActionResult> SendMailAsync([FromForm] MailingCreateDto mailRequestDto)
        {
            if (string.IsNullOrEmpty(mailRequestDto.ToEmail) && !mailRequestDto.ToMailingListId.HasValue)
            {
                return BadRequest("ToEmail or ToMailingListId Parameter is required.");
            }

            try
            {
                MailRequest mailRequest = _mapper.Map<MailRequest>(mailRequestDto);

                await _mailingService.SendMailingAsync(mailRequest, mailRequestDto);
                
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
        public IActionResult AddRecipientToMailingList([FromForm] RecipientCreateDto recipientCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return _mailingService.CreateRecipientForMailingList(recipientCreateDto) ? Ok() : BadRequest();
        }
    }
}

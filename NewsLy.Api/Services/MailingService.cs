using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using NewsLy.Api.Models;
using NewsLy.Api.Settings;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services.Interfaces;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Dtos.Recipient;
using AutoMapper;
using NewsLy.Api.Enums;

namespace NewsLy.Api.Services
{
    public class MailingService : IMailingService
    {
        private readonly ILogger<MailingService> _logger;
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _configuration;
        private readonly IMailRequestRepository _mailRequestRepository;
        private readonly IMailingListRepository _mailingListRepository;
        private readonly IRecipientRepository _recipientRepository;
        private readonly ITrackingService _trackingService;
        private readonly IMapper _mapper;

        public MailingService(
            ILogger<MailingService> logger,
            IOptions<MailSettings> mailSettings,
            IConfiguration configuration,
            IMailRequestRepository mailRequestRepository,
            IMailingListRepository mailingListRepository,
            IRecipientRepository recipientRepository,
            ITrackingService trackingService,
            IMapper mapper
        )
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
            _configuration = configuration;
            _mailRequestRepository = mailRequestRepository;
            _mailingListRepository = mailingListRepository;
            _recipientRepository = recipientRepository;
            _trackingService = trackingService;
            _mapper = mapper;
        }

        public async Task<MailRequest> SendMailingAsync(MailingCreateDto mailingCreateDto, MailType mailType)
        {
            var email = new MimeMessage();
            email.Importance = MessageImportance.Normal;
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.SenderMail));
            email.Bcc.AddRange(GetMailingRecipients(mailingCreateDto));

            email.Subject = mailingCreateDto.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = await BuildEmailHtmlBody(mailingCreateDto, mailType);

            if(string.IsNullOrEmpty(bodyBuilder.HtmlBody))
            {
                return null;
            }

            TryAddAttachments(mailingCreateDto, bodyBuilder);

            email.Body = bodyBuilder.ToMessageBody();

            await SendMailAsync(email);

            var mailing = _mapper.Map<MailRequest>(mailingCreateDto);            

            _mailRequestRepository.Add(mailing);

            return mailing;
        }

        public IEnumerable<MailingListDto> GetAllMailingLists()
        {
            var allMailingLists = _mailingListRepository.GetAllActive();

            List<MailingListDto> mailingListDtos = new();

            foreach (var mailingList in allMailingLists)
            {
                var mailingListDto = new MailingListDto {
                    Name = mailingList.Name,
                    AmountOfRecipients = 
                        _recipientRepository
                        .GetAmountOfRecipientsForMailingList(mailingList.Id)
                };

                mailingListDtos.Add(mailingListDto);
            }

            return mailingListDtos;
        }

        public IEnumerable<MailRequestDto> GetAllMailRequests()
        {
            return _mapper.Map<List<MailRequestDto>>(_mailRequestRepository.GetAll());
        }

        public async Task<bool> CreateRecipientForMailingList(RecipientCreateDto recipientCreateDto)
        {
            var mailingList = _mailingListRepository.Find(recipientCreateDto.MailingListId);

            if (mailingList == null)
            {
                _logger.LogWarning($"MailingList with Id { recipientCreateDto.MailingListId } does not exist.");
                return false;
            }

            if (_recipientRepository.FindByEmailAndMailingList(recipientCreateDto.Email, recipientCreateDto.MailingListId) != null)
            {
                _logger.LogWarning("Recipient with Email already exists");
                return false;
            }

            var tempRecipient = _mapper.Map<Recipient>(recipientCreateDto);
            tempRecipient.VerificationToken = GenerateNewVerifikationToken();

            var newRecipient = _recipientRepository.Add(
                tempRecipient,
                recipientCreateDto.MailingListId
            );

            if (newRecipient == null)
            {
                return false;
            }

            await SendMailingAsync(
                new MailingCreateDto
                {
                    ToEmail = newRecipient.Email,
                    ToMailingListId = null,
                    Subject = "Newsletteranmeldung bestätigen",
                    TrackLinks = false,
                    VerificationLink = newRecipient.VerificationToken
                },
                MailType.DoubleOptIn
            );

            newRecipient.ConfirmationMailSentDate = DateTime.Now;
            _recipientRepository.Update(newRecipient);

            return newRecipient != null;
        }

        public async Task<bool> VerifyRecipientEmail(string verificationToken)
        {
            var recipient = _recipientRepository.FindByVerificationToken(verificationToken);

            if (recipient == null)
            {
                return false;
            }

            recipient.ConfirmationDate = DateTime.Now;
            recipient.IsVerified = true;
            recipient.VerificationToken = "";

            if (_recipientRepository.Update(recipient) != null)
            {
                await SendMailingAsync(
                    new MailingCreateDto {
                        ToEmail = recipient.Email,
                        Name = recipient.Firstname,
                        Subject = "Willkommen",
                        TrackLinks = false
                    },
                    MailType.Welcome
                );

                return true;
            }

            return false;
        }


        private IEnumerable<InternetAddress> GetMailingRecipients(MailingCreateDto mailingCreateDto)
        {
            var internetAddresses = new List<InternetAddress>();

            if (!string.IsNullOrEmpty(mailingCreateDto.ToEmail))
            {
                internetAddresses.Add(MailboxAddress.Parse(mailingCreateDto.ToEmail));
            }

            if (mailingCreateDto.ToMailingListId.HasValue &&
                _mailingListRepository.Find((int) mailingCreateDto.ToMailingListId) != null
            )
            {
                var recipients = _recipientRepository.GetAllFromMailingList((int) mailingCreateDto.ToMailingListId);

                internetAddresses.AddRange(recipients.Select(x => MailboxAddress.Parse(x.Email)));
            }

            return internetAddresses;
        }

        private async Task SendMailAsync(MimeMessage mimeMessage)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Username, _mailSettings.Password);

            await smtp.SendAsync(mimeMessage);

            smtp.Disconnect(true);
        }

        private async Task<string> BuildEmailHtmlBody(MailingCreateDto mailingCreateDto, MailType mailType)
        {
            var emailVariables = new List<Tuple<string, string>>
            {
                Tuple.Create("Title", mailingCreateDto.Subject),
                Tuple.Create("Subject", mailingCreateDto.Subject),
                Tuple.Create("Name", mailingCreateDto.Name),
                Tuple.Create("Email", mailingCreateDto.ToEmail),
                Tuple.Create("ApplicationUrl", _configuration["ApplicationDomain"]),
                Tuple.Create("CurrentYear", DateTime.Now.ToString("yyyy"))
            };

            var mailTemplateName = "MailTemplate";

            switch (mailType)
            {
                case MailType.ContactRequest:
                    emailVariables.Add(Tuple.Create("Message", mailingCreateDto.Message));
                    break;
                case MailType.DoubleOptIn:
                    mailTemplateName = "DoubleOptInTemplate";
                    emailVariables.Add(
                        Tuple.Create(
                            "VerificationLink",
                            $"{ _configuration["ApplicationDomain"] }/api/mailings/recipient-verification?token={ mailingCreateDto.VerificationLink }"
                        )
                    );
                    break;
                case MailType.Welcome:
                    mailTemplateName = "WelcomeTemplate";
                    break;
                default:
                    break;
            }

            string mailTemplate = await GetMailTemplate(
                mailTemplateName,
                mailingCreateDto.HtmlContent
            );

            if (!string.IsNullOrEmpty(mailTemplate))
            {
                string mailContent = ReplaceVariables(
                    mailTemplate,
                    emailVariables
                );

                if (mailingCreateDto.TrackLinks)
                {
                    return _trackingService.DetectCreateAndReplaceTrackings(mailContent);
                }

                return mailContent;
            }

            return "";
        }

        private async Task<string> GetMailTemplate(string mailTemplateName, string customHtmlContent = "") 
        {
            var mailTemplate = "";

            if (!string.IsNullOrEmpty(customHtmlContent))
            {
                return customHtmlContent;
            }

            try
            {
                var streamReader = new StreamReader(
                    Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{ mailTemplateName }.html")
                );

                mailTemplate = await streamReader.ReadToEndAsync();

                streamReader.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }


            return mailTemplate;
        }

        private static string ReplaceVariables(string mailTemplate, IEnumerable<Tuple<string, string>> mailReplaceParameters)
        {
            var mailContent = new StringBuilder(mailTemplate);

            foreach (var mailReplaceParameter in mailReplaceParameters)
            {
                mailContent.Replace(
                    "{{" + mailReplaceParameter.Item1 + "}}",
                    mailReplaceParameter.Item2
                );
            }

            return mailContent.ToString();
        }

        private static void TryAddAttachments(MailingCreateDto mailingCreateDto, BodyBuilder bodyBuilder)
        {
            if (mailingCreateDto.Attachments != null)
            {
                byte[] fileBytes;

                foreach (var file in mailingCreateDto.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }

                        bodyBuilder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
        }
   
        private string GenerateNewVerifikationToken()
        {
            var random = new Random();
            const string characters = "abcdefghijklmnopqrstuvwxyz";
            int.TryParse(
                _configuration["EmailVerificationTokenLength"],
                out var emailVerificationTokenLength
            );

            return new string(
                Enumerable.Repeat(characters, emailVerificationTokenLength)
                .Select(x => x[random.Next(x.Length)])
                .ToArray()
            );
        }
    }
}

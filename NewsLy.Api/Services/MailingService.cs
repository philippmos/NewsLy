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

namespace NewsLy.Api.Services
{
    public class MailingService : IMailingService
    {
        private readonly ILogger<MailingService> _logger;
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _configuration;
        private readonly IMailingListRepository _mailingListRepository;
        private readonly IRecipientRepository _recipientRepository;
        private readonly ITrackingService _trackingService;

        public MailingService(
            ILogger<MailingService> logger,
            IOptions<MailSettings> mailSettings,
            IConfiguration configuration,
            IMailingListRepository mailingListRepository,
            IRecipientRepository recipientRepository,
            ITrackingService trackingService
        )
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
            _configuration = configuration;
            _mailingListRepository = mailingListRepository;
            _recipientRepository = recipientRepository;
            _trackingService = trackingService;
        }

        public async Task SendMailingAsync(ContactRequest mailRequest, MailingCreateDto mailingCreateDto)
        {
            var email = new MimeMessage();
            email.Importance = MessageImportance.Normal;
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.SenderMail));
            email.Bcc.AddRange(GetMailingRecipients(mailRequest));

            email.Subject = mailRequest.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = BuildEmailHtmlBody(mailRequest, mailingCreateDto);

            if(string.IsNullOrEmpty(bodyBuilder.HtmlBody))
            {
                return;
            }

            TryAddAttachments(mailRequest, bodyBuilder);

            email.Body = bodyBuilder.ToMessageBody();

            await SendMailAsync(email);
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

        public bool CreateRecipientForMailingList(RecipientCreateDto recipientCreateDto)
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

            var newRecipient = _recipientRepository.Add(
                new Recipient
                {
                    Firstname = recipientCreateDto.Firstname,
                    Lastname = recipientCreateDto.Lastname,
                    Email = recipientCreateDto.Email
                },
                recipientCreateDto.MailingListId
            );

            return newRecipient != null;
        }


        private IEnumerable<InternetAddress> GetMailingRecipients(ContactRequest mailRequest)
        {
            var internetAddresses = new List<InternetAddress>();

            if (!string.IsNullOrEmpty(mailRequest.ToEmail))
            {
                internetAddresses.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            }

            if (mailRequest.ToMailingListId.HasValue &&
                _mailingListRepository.Find((int) mailRequest.ToMailingListId) != null
            )
            {
                var recipients = _recipientRepository.GetAllFromMailingList((int) mailRequest.ToMailingListId);

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

        private string BuildEmailHtmlBody(ContactRequest mailRequest, MailingCreateDto mailingCreateDto)
        {
            string mailTemplate = "";

            if (!string.IsNullOrEmpty(mailingCreateDto.HtmlContent))
            {
                mailTemplate = mailingCreateDto.HtmlContent;
            }
            else
            {
                try
                {
                    var streamReader = new StreamReader(
                        Path.Combine(Directory.GetCurrentDirectory(), "Templates", "MailTemplate.html")
                    );

                    mailTemplate = streamReader.ReadToEnd();

                    streamReader.Close();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(mailTemplate))
            {
                string mailContent = ReplaceVariables(
                    mailTemplate,
                    new[]
                    {
                        Tuple.Create("Title", mailRequest.Subject),
                        Tuple.Create("Subject", mailRequest.Subject),
                        Tuple.Create("Name", mailRequest.Name),
                        Tuple.Create("Email", mailRequest.ToEmail),
                        Tuple.Create("RequestIp", mailRequest.RequestIp),
                        Tuple.Create("Message", mailRequest.Message),
                        Tuple.Create("ApplicationUrl", _configuration["ApplicationDomain"])
                    }
                );

                if (mailingCreateDto.TrackLinks)
                {
                    return _trackingService.DetectCreateAndReplaceTrackings(mailContent);
                }

                return mailContent;
            }

            return "";
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

        private static void TryAddAttachments(MailRequest mailRequest, BodyBuilder bodyBuilder)
        {
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;

                foreach (var file in mailRequest.Attachments)
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
    }
}

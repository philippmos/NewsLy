using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using NewsLy.Api.Models;
using NewsLy.Api.Settings;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Services
{
    public class MailingService : IMailingService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailingService> _logger;
        private readonly IMailingListRepository _mailingListRepository;
        private readonly IRecipientRepository _recipientRepository;

        public MailingService(
            ILogger<MailingService> logger,
            IOptions<MailSettings> mailSettings,
            IMailingListRepository mailingListRepository,
            IRecipientRepository recipientRepository
        )
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
            _mailingListRepository = mailingListRepository;
            _recipientRepository = recipientRepository;
        }

        public async Task SendMailingAsync(ContactRequest mailRequest)
        {
            var mailSubject = "New Contact Request";

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.Bcc.AddRange(GetMailingRecipients(mailRequest));

            email.Subject = mailSubject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = BuildEmailHtmlBody(mailRequest, mailSubject);

            if(string.IsNullOrEmpty(bodyBuilder.HtmlBody))
            {
                return;
            }

            TryAddAttachments(mailRequest, bodyBuilder);

            email.Body = bodyBuilder.ToMessageBody();

            await SendMailAsync(email);
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
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(mimeMessage);

            smtp.Disconnect(true);
        }

        private string BuildEmailHtmlBody(ContactRequest mailRequest, string mailSubject)
        {
            string mailTemplate = "";

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

            if (!string.IsNullOrEmpty(mailTemplate))
            {
                return ReplaceVariables(
                    mailTemplate,
                    new[]
                    {
                        Tuple.Create("Title", mailSubject),
                        Tuple.Create("Subject", mailSubject),
                        Tuple.Create("Name", mailRequest.Name),
                        Tuple.Create("Email", mailRequest.ToEmail),
                        Tuple.Create("RequestIp", mailRequest.RequestIp),
                        Tuple.Create("Message", mailRequest.Message),
                        Tuple.Create("ApplicationUrl", "http://localhost")
                    }
                );
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

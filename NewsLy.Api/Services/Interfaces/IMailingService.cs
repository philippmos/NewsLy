using System.Collections.Generic;
using System.Threading.Tasks;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Dtos.Recipient;
using NewsLy.Api.Enums;
using NewsLy.Api.Models;

namespace NewsLy.Api.Services.Interfaces
{
    public interface IMailingService
    {
        Task<MailRequest> SendMailingAsync(MailingCreateDto mailingCreateDto, MailType mailType);
        IEnumerable<MailingListDto> GetAllMailingLists();
        Task<bool> CreateRecipientForMailingList(RecipientCreateDto recipientCreateDto);
        Task<bool> VerifyRecipientEmail(string verificationToken);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Dtos.Recipient;
using NewsLy.Api.Enums;

namespace NewsLy.Api.Services.Interfaces
{
    public interface IMailingService
    {
        Task<bool> SendMailingAsync(MailingCreateDto mailingCreateDto, MailType mailType);
        IEnumerable<MailingListDto> GetAllMailingLists();
        IEnumerable<MailRequestDto> GetAllMailRequests();
        Task<bool> CreateRecipientForMailingList(RecipientCreateDto recipientCreateDto);
        Task<bool> VerifyRecipientEmail(string verificationToken);
    }
}
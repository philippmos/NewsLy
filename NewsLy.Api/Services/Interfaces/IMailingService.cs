using System.Collections.Generic;
using System.Threading.Tasks;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Dtos.Recipient;
using NewsLy.Api.Models;

namespace NewsLy.Api.Services.Interfaces
{
    public interface IMailingService
    {
        Task SendMailingAsync(MailRequest mailRequest, MailingCreateDto mailingCreateDto);
        IEnumerable<MailingListDto> GetAllMailingLists();
        bool CreateRecipientForMailingList(RecipientCreateDto recipientCreateDto);
    }
}
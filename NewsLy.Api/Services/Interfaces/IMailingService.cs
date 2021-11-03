using System.Collections.Generic;
using System.Threading.Tasks;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Models;

namespace NewsLy.Api.Services.Interfaces
{
    public interface IMailingService
    {
        Task SendMailingAsync(ContactRequest mailRequest, bool trackLinks);
        IEnumerable<MailingListDto> GetAllMailingLists();
    }
}
using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories.Interfaces
{
    public interface IMailingListRepository
    {
        MailingList Find(int id);
        IEnumerable<MailingList> GetAll();
        IEnumerable<MailingList> GetAllActive();

        MailingList Add(MailingList mailingList);
    }
}
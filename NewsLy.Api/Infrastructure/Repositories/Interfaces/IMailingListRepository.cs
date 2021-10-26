using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Infrastructure.Repositories.Interfaces
{
    public interface IMailingListRepository
    {
        MailingList Find(int id);
        List<MailingList> GetAll();

        MailingList Add(MailingList mailingList);
    }
}
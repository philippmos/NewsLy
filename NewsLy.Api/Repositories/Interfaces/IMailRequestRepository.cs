using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories.Interfaces
{
    public interface IMailRequestRepository
    {
        MailRequest Find(int id);
        List<MailRequest> GetAll();

        MailRequest Add(MailRequest contactRequest);
    }
}
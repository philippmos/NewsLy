using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Infrastructure.Repositories.Interfaces
{
    public interface IRecipientRepository
    {
        Recipient Find(int id);
        List<Recipient> GetAllFromMailingList(int mailingListId);

        Recipient Add(Recipient recipient, int mailingListId);
    }
}
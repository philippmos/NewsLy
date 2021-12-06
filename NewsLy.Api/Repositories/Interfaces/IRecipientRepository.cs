using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories.Interfaces
{
    public interface IRecipientRepository
    {
        Recipient Find(int id);
        Recipient FindByEmailAndMailingList(string email, int mailingListId);
        List<Recipient> GetAllFromMailingList(int mailingListId);
        int GetAmountOfRecipientsForMailingList(int mailingListId);

        Recipient Add(Recipient recipient, int mailingListId);
    }
}
using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories.Interfaces
{
    public interface IRecipientRepository
    {
        Recipient Find(int id);
        Recipient FindByEmailAndMailingList(string email, int mailingListId);
        Recipient FindByVerificationToken(string token);
        IEnumerable<Recipient> GetAllFromMailingList(int mailingListId);
        IEnumerable<Recipient> GetAllVerifiedFromMailingList(int mailingListId);
        int GetAmountOfRecipientsForMailingList(int mailingListId);

        Recipient Add(Recipient recipient, int mailingListId);
        Recipient Update(Recipient recipient, int mailingListId = 0);
    }
}
using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories
{
    public interface IContactRequestRepository
    {
        ContactRequest Find(int id);
        List<ContactRequest> GetAll();

        ContactRequest Add(ContactRequest contactRequest);
    }
}
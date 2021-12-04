using System.Collections.Generic;
using System.Linq;
using NewsLy.Api.Data;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Repositories.EntityFramework
{
    public class ContactRequestRepository : IContactRequestRepository
    {
        private ApplicationDbContext _dbcontext;

        public ContactRequestRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public ContactRequest Find(int id) 
        {
            return _dbcontext.ContactRequests.FirstOrDefault(x => x.Id == id);
        }

        public List<ContactRequest> GetAll()
        {
            return _dbcontext.ContactRequests.ToList();
        }

        public ContactRequest Add(ContactRequest contactRequest)
        {
            _dbcontext.ContactRequests.Add(contactRequest);
            _dbcontext.SaveChanges();

            return contactRequest;
        }
    }
}
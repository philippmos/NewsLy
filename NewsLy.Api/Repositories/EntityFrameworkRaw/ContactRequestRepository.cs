using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NewsLy.Api.Data;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Repositories.EntityFrameworkRaw
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
            return _dbcontext
                        .ContactRequests
                        .FromSqlRaw("SELECT TOP(1) * FROM ContactRequests WHERE Id = {0}", id)
                        .FirstOrDefault();
        }

        public List<ContactRequest> GetAll()
        {
            return _dbcontext
                        .ContactRequests
                        .FromSqlRaw("SELECT * FROM ContactRequests;")
                        .ToList();
        }

        public ContactRequest Add(ContactRequest contactRequest)
        {
            _dbcontext.ContactRequests.Add(contactRequest);
            _dbcontext.SaveChanges();

            return contactRequest;
        }
    }
}
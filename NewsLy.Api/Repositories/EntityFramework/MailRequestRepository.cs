using System;
using System.Collections.Generic;
using System.Linq;
using NewsLy.Api.Data;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Repositories.EntityFramework
{
    public class MailRequestRepository : IMailRequestRepository
    {
        private ApplicationDbContext _dbcontext;

        public MailRequestRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public MailRequest Find(int id) 
        {
            return _dbcontext.MailRequests.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<MailRequest> GetAll()
        {
            return _dbcontext.MailRequests.ToList();
        }

        public MailRequest Add(MailRequest mailRequest)
        {
            mailRequest.CreationDate = DateTime.Now;
            _dbcontext.MailRequests.Add(mailRequest);
            _dbcontext.SaveChanges();

            return mailRequest;
        }
    }
}
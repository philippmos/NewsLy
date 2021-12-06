using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NewsLy.Api.Data;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Repositories.EntityFrameworkRaw
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
            return _dbcontext
                        .MailRequests
                        .FromSqlRaw("SELECT TOP(1) * FROM MailRequests WHERE Id = {0}", id)
                        .FirstOrDefault();
        }

        public List<MailRequest> GetAll()
        {
            return _dbcontext
                        .MailRequests
                        .FromSqlRaw("SELECT * FROM MailRequests;")
                        .ToList();
        }

        public MailRequest Add(MailRequest MailRequest)
        {
            _dbcontext.MailRequests.Add(MailRequest);
            _dbcontext.SaveChanges();

            return MailRequest;
        }
    }
}
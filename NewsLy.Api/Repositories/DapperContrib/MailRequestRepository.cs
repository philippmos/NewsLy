using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Repositories.DapperContrib
{
    public class MailRequestRepository : IMailRequestRepository
    {
        private IDbConnection _dbconnection;

        public MailRequestRepository(IConfiguration configuration)
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public MailRequest Find(int id) 
        {
            return _dbconnection.Get<MailRequest>(id);
        }

        public IEnumerable<MailRequest> GetAll()
        {
            return _dbconnection.GetAll<MailRequest>().ToList();
        }

        public MailRequest Add(MailRequest mailRequest)
        {
            mailRequest.CreationDate = DateTime.Now;
            mailRequest.Id = (int)_dbconnection.Insert(mailRequest);

            return mailRequest;
        }
    }
}
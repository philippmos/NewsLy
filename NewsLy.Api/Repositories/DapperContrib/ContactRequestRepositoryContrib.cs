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
    public class ContactRequestRepository : IContactRequestRepository
    {
        private IDbConnection _dbconnection;

        public ContactRequestRepository(IConfiguration configuration)
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public ContactRequest Find(int id) 
        {
            return _dbconnection.Get<ContactRequest>(id);
        }

        public List<ContactRequest> GetAll()
        {
            return _dbconnection.GetAll<ContactRequest>().ToList();
        }

        public ContactRequest Add(ContactRequest contactRequest)
        {
            contactRequest.Id = (int)_dbconnection.Insert(contactRequest);

            return contactRequest;
        }
    }
}
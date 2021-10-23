using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories
{
    public class ContactRequestRepositoryContrib : IContactRequestRepository
    {
        private IDbConnection _dbconnection;

        public ContactRequestRepositoryContrib(IConfiguration configuration)
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
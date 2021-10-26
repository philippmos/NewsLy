using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NewsLy.Api.Models;
using NewsLy.Api.Infrastructure.Repositories.Interfaces;

namespace NewsLy.Api.Infrastructure.Repositories.Dapper
{
    public class ContactRequestRepository : IContactRequestRepository
    {
        private IDbConnection _dbconnection;

        private readonly string _repoTableName = "ContactRequests";

        public ContactRequestRepository(
            IConfiguration configuration
        )
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public ContactRequest Find(int id) 
        {
            var sqlQuery = $"SELECT TOP(1) FROM { _repoTableName } WHERE Id = @Id";

            return _dbconnection.Query<ContactRequest>(sqlQuery, new { @Id = id }).Single();
        }

        public List<ContactRequest> GetAll()
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName }";

            return _dbconnection.Query<ContactRequest>(sqlQuery).ToList();
        }

        public ContactRequest Add(ContactRequest contactRequest)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"INSERT INTO { _repoTableName }");
            sqlQuery.Append(" (Message, ToEmail, Name, Subject)");
            sqlQuery.Append(" VALUES (@Message, @ToEmail, @Name, @Subject);");
            sqlQuery.Append(" SELECT CAST(SCOPE_IDENTITY() as int);");

            contactRequest.Id = _dbconnection.Query<int>(sqlQuery.ToString(), contactRequest).Single();

            return contactRequest;
        }
    }
}
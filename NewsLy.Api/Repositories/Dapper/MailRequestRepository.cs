using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Repositories.Dapper
{
    public class MailRequestRepository : IMailRequestRepository
    {
        private IDbConnection _dbconnection;

        private readonly string _repoTableName = "MailRequests";

        public MailRequestRepository(
            IConfiguration configuration
        )
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public MailRequest Find(int id) 
        {
            var sqlQuery = $"SELECT TOP(1) FROM { _repoTableName } WHERE Id = @Id";

            return _dbconnection.Query<MailRequest>(sqlQuery, new { @Id = id }).FirstOrDefault();
        }

        public IEnumerable<MailRequest> GetAll()
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName }";

            return _dbconnection.Query<MailRequest>(sqlQuery).ToList();
        }

        public MailRequest Add(MailRequest mailRequest)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"INSERT INTO { _repoTableName }");
            sqlQuery.Append(" (Message, ToEmail, Name, Subject, CreationDate)");
            sqlQuery.Append($" VALUES (@Message, @ToEmail, @Name, @Subject, { DateTime.Now });");
            sqlQuery.Append(" SELECT CAST(SCOPE_IDENTITY() as int);");

            mailRequest.Id = _dbconnection.Query<int>(sqlQuery.ToString(), mailRequest).FirstOrDefault();

            return mailRequest;
        }
    }
}
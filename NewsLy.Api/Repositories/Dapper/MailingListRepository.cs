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
    public class MailingListRepository : IMailingListRepository
    {
        private IDbConnection _dbconnection;

        private readonly string _repoTableName = "MailingLists";

        public MailingListRepository(
            IConfiguration configuration
        )
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public MailingList Find(int id) 
        {
            var sqlQuery = $"SELECT TOP(1) FROM { _repoTableName } WHERE Id = @Id";

            return _dbconnection.Query<MailingList>(sqlQuery, new { @Id = id }).Single();
        }

        public List<MailingList> GetAll()
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName }";

            return _dbconnection.Query<MailingList>(sqlQuery).ToList();
        }

        public MailingList Add(MailingList mailingList)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"INSERT INTO { _repoTableName }");
            sqlQuery.Append(" (Name, IsActive)");
            sqlQuery.Append(" VALUES (@Name, @IsActive);");
            sqlQuery.Append(" SELECT CAST(SCOPE_IDENTITY() as int);");

            mailingList.Id = _dbconnection.Query<int>(sqlQuery.ToString(), mailingList).Single();

            return mailingList;
        }
    }
}
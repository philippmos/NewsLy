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
    public class RecipientRepository : IRecipientRepository
    {
        private IDbConnection _dbconnection;

        private readonly string _repoTableName = "Recipients";

        public RecipientRepository(
            IConfiguration configuration
        )
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Recipient Find(int id) 
        {
            var sqlQuery = $"SELECT TOP(1) FROM { _repoTableName } WHERE Id = @Id";

            return _dbconnection.Query<Recipient>(sqlQuery, new { @Id = id }).Single();
        }

        public List<Recipient> GetAllFromMailingList(int mailingListId)
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE MailingListId = @MailingListId";

            return _dbconnection.Query<Recipient>(sqlQuery, new { @MailingListId = mailingListId }).ToList();
        }

        public Recipient Add(Recipient recipient, int mailingListId)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"INSERT INTO { _repoTableName }");
            sqlQuery.Append(" (Firstname, Lastname, Email, Gender, MailingListId)");
            sqlQuery.Append(" VALUES (@Firstname, @Lastname, @Email, @Gender, @MailingListId);");
            sqlQuery.Append(" SELECT CAST(SCOPE_IDENTITY() as int);");

            recipient.Id = _dbconnection.Query<int>(
                sqlQuery.ToString(),
                new 
                { 
                    @Firstname = recipient.Firstname,
                    @Lastname = recipient.Lastname,
                    @Email = recipient.Email,
                    @Gender = recipient.Gender,
                    @MailingListId = mailingListId
                }
            ).Single();

            return recipient;
        }
    }
}
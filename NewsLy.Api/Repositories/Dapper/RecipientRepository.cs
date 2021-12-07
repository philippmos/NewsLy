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
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE Id = @Id";

            return _dbconnection.Query<Recipient>(sqlQuery, new { @Id = id }).FirstOrDefault();
        }

        public Recipient FindByEmailAndMailingList(string email, int mailingListId)
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE Email = @Email AND MailingListId = @MailingListId";

            return _dbconnection.Query<Recipient>(sqlQuery, new { @Email = email, @MailingListId = mailingListId}).FirstOrDefault();
        }

        public Recipient FindByVerificationToken(string token)
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE VerificationToken = @VerificationToken";

            return _dbconnection.Query<Recipient>(sqlQuery, new { @VerificationToken = token }).FirstOrDefault();
        }

        public IEnumerable<Recipient> GetAllFromMailingList(int mailingListId)
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE MailingListId = @MailingListId";

            return _dbconnection.Query<Recipient>(sqlQuery, new { @MailingListId = mailingListId }).ToList();
        }
        
        public IEnumerable<Recipient> GetAllVerifiedFromMailingList(int mailingListId)
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE MailingListId = @MailingListId AND IsVerified = @IsVerified";

            return _dbconnection.Query<Recipient>
                        (
                            sqlQuery,
                            new 
                            {
                                @MailingListId = mailingListId,
                                @IsVerified = true
                            }
                        ).ToList();
        }

        public int GetAmountOfRecipientsForMailingList(int mailingListId)
        {
            return _dbconnection.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM { _repoTableName } WHERE MailingListId = @MailingListId",
                new { @MailingListId = mailingListId }
            );
        }

        public Recipient Add(Recipient recipient, int mailingListId)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"INSERT INTO { _repoTableName }");
            sqlQuery.Append(" (Firstname, Lastname, Email, Gender, MailingListId, IsVerified, VerificationToken, CreationDate)");
            sqlQuery.Append(" VALUES (@Firstname, @Lastname, @Email, @Gender, @MailingListId, @IsVerified, @VerificationToken, @CreationDate);");
            sqlQuery.Append(" SELECT CAST(SCOPE_IDENTITY() as int);");

            recipient.Id = _dbconnection.Query<int>(
                sqlQuery.ToString(),
                new 
                { 
                    @Firstname = recipient.Firstname,
                    @Lastname = recipient.Lastname,
                    @Email = recipient.Email,
                    @Gender = recipient.Gender,
                    @MailingListId = mailingListId,
                    @IsVerified = recipient.IsVerified,
                    @VerificationToken = recipient.VerificationToken,
                    @CreationDate = DateTime.Now
                }
            ).FirstOrDefault();

            return recipient;
        }

        public Recipient Update(Recipient recipient, int mailingListId = 0)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"UPDATE { _repoTableName }");
            sqlQuery.Append(" SET Firstname = @Firstname, Lastname = @Lastname, Email = @Email, Gender = @Gender,");
            sqlQuery.Append(" ConfirmationMailSentDate = @ConfirmationMailSentDate, ConfirmationDate = @ConfirmationDate,");
            sqlQuery.Append(" IsVerified = @IsVerified, VerificationToken = @VerificationToken, ModificationDate = @ModificationDate");
            
            if (mailingListId > 0)
            {
                sqlQuery.Append(", MailingListId = @MailingListId");
            }

            sqlQuery.Append(" WHERE Id = @Id");

            _dbconnection.Execute(
                sqlQuery.ToString(),
                new 
                { 
                    @Id = recipient.Id,
                    @Firstname = recipient.Firstname,
                    @Lastname = recipient.Lastname,
                    @Email = recipient.Email,
                    @Gender = recipient.Gender,
                    @MailingListId = mailingListId,
                    @ConfirmationMailSentDate = recipient.ConfirmationMailSentDate,
                    @ConfirmationDate = recipient.ConfirmationDate,
                    @IsVerified = recipient.IsVerified,
                    @VerificationToken = recipient.VerificationToken,
                    @ModificationDate = DateTime.Now
                }
            );

            return recipient;
        }
    }
}
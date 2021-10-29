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
    public class TrackingUrlRepository : ITrackingUrlRepository
    {
        private IDbConnection _dbconnection;

        private readonly string _repoTableName = "TrackingUrls";

        public TrackingUrlRepository(
            IConfiguration configuration
        )
        {
            _dbconnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public TrackingUrl FindByTrackingId(string trackingId) 
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName } WHERE TrackingId = @TrackingId";

            return _dbconnection.Query<TrackingUrl>(sqlQuery, new { @TrackingId = trackingId }).Single();
        }

        public List<TrackingUrl> GetAll()
        {
            var sqlQuery = $"SELECT * FROM { _repoTableName }";

            return _dbconnection.Query<TrackingUrl>(sqlQuery).ToList();
        }

        public List<TrackingUrl> GetAllActive()
        {
            return _dbconnection.Query<TrackingUrl>(
                $"SELECT * FROM { _repoTableName } WHERE IsActive = @IsActive",
                new { @IsActive = true }
            ).ToList();
        }

        public TrackingUrl Add(TrackingUrl trackingUrl)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"INSERT INTO { _repoTableName }");
            sqlQuery.Append(" (TrackingId, TargetUrl, AccessCount, IsActive)");
            sqlQuery.Append(" VALUES (@TrackingId, @TargetUrl, @AccessCount, @IsActive);");
            sqlQuery.Append(" SELECT CAST(SCOPE_IDENTITY() as int);");

            trackingUrl.Id = _dbconnection.Query<int>(sqlQuery.ToString(), trackingUrl).Single();

            return trackingUrl;
        }

        public TrackingUrl Update(TrackingUrl trackingUrl)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append($"UPDATE { _repoTableName }");
            sqlQuery.Append(" SET TrackingId = @TrackingId, TargetUrl = @TargetUrl, AccessCount = @AccessCount, IsActive = @IsActive");
            sqlQuery.Append(" WHERE Id = @Id");

            _dbconnection.Execute(sqlQuery.ToString(), trackingUrl);

            return trackingUrl;
        }
    }
}
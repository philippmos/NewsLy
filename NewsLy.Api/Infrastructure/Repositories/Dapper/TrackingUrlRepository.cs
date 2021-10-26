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
    }
}
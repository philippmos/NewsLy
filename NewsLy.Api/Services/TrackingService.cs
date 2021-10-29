using NewsLy.Api.Services.Interfaces;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;

namespace NewsLy.Api.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly IConfiguration _configuration;

        public TrackingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateTrackingId()
        {
            int.TryParse(_configuration["TrackingIdLength"], out int trackingIdLength);
            
            var random = new Random();
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(characters, trackingIdLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }      
    }
}

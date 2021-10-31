using NewsLy.Api.Services.Interfaces;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NewsLy.Api.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ILogger<TrackingService> _logger;
        private readonly IConfiguration _configuration;

        public TrackingService(
            ILogger<TrackingService> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
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

        public string DetectCreateAndReplaceTrackings(string inputString)
        {
            var foundLinks = FindLinkMatchesInString(inputString);

            return inputString;
        }

        private IEnumerable<string> FindLinkMatchesInString(string inputString)
        {
            List<string> foundLinksList = new();

            foreach (Match aTagMatch in Regex.Matches(inputString, @"(<a.*?>.*?</a>)"))
            {
                var matchValue = aTagMatch.Groups[1].Value;

                Match hrefValueMatch = Regex.Match(matchValue, @"href=\""(?!mailto:)(.*?)\""");

                if (hrefValueMatch.Success)
                {
                    foundLinksList.Add(hrefValueMatch.Groups[1].Value);
                }
            }

            return foundLinksList;
        }
    }
}

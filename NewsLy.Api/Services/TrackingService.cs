using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NewsLy.Api.Services.Interfaces;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;

namespace NewsLy.Api.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ILogger<TrackingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITrackingUrlRepository _trackingUrlRepository;

        public TrackingService(
            ILogger<TrackingService> logger,
            IConfiguration configuration,
            ITrackingUrlRepository trackingUrlRepository
        )
        {
            _logger = logger;
            _configuration = configuration;
            _trackingUrlRepository = trackingUrlRepository;
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
            var newTrackingLinks = CreateTrackingLinks(
                FindLinkMatchesInString(inputString)
            );

            return ReplaceAllTrackingLinks(inputString, newTrackingLinks);
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


        private IEnumerable<TrackingUrl> CreateTrackingLinks(IEnumerable<string> plainLinks)
        {
            List<TrackingUrl> newTrackingUrls = new();

            foreach (var plainLink in plainLinks)
            {
                newTrackingUrls.Add(
                    _trackingUrlRepository.Add(
                        new TrackingUrl {
                            TargetUrl = plainLink,
                            TrackingId = GenerateTrackingId(),
                            IsActive = true,
                            AccessCount = 0
                        }
                    )
                );
            }

            return newTrackingUrls;
        }

        private string ReplaceAllTrackingLinks(string inputString, IEnumerable<TrackingUrl> trackingUrls)
        {
            StringBuilder mailContent = new(inputString);

            foreach(var trackingUrl in trackingUrls)
            {
                mailContent.Replace(
                    trackingUrl.TargetUrl,
                    $"{ _configuration["ApplicationDomain"] }/api/tracking/redirection?t={ trackingUrl.TrackingId }"
                );
            }

            return mailContent.ToString();
        }
    }
}
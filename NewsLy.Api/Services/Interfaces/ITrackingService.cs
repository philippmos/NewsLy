using System.Collections.Generic;
using NewsLy.Api.Dtos.Tracking;
using NewsLy.Api.Models;

namespace NewsLy.Api.Services.Interfaces
{
    public interface ITrackingService
    {
        string GenerateTrackingId();
        string DetectCreateAndReplaceTrackings(string inputString);
        string GetTargetUrlAndIncreaseAccessCount(string trackingId);
        IEnumerable<TrackingUrlDto> GetAllActive();
        TrackingUrl CreateNewTrackingUrl(TrackingUrlCreateDto trackingUrlCreateDto);
    }
}
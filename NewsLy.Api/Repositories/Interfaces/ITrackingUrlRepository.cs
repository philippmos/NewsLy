using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Repositories.Interfaces
{
    public interface ITrackingUrlRepository
    {
        TrackingUrl FindByTrackingId(string trackingId);
        List<TrackingUrl> GetAll();
        List<TrackingUrl> GetAllActive();
        TrackingUrl Add(TrackingUrl trackingUrl);
        TrackingUrl Update(TrackingUrl trackingUrl);
    }
}
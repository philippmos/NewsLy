using System.Collections.Generic;
using NewsLy.Api.Models;

namespace NewsLy.Api.Infrastructure.Repositories.Interfaces
{
    public interface ITrackingUrlRepository
    {
        TrackingUrl FindByTrackingId(string trackingId);
        List<TrackingUrl> GetAll();
    }
}
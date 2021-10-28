using AutoMapper;
using NewsLy.Api.Dtos.Tracking;
using NewsLy.Api.Models;

namespace NewsLy.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TrackingUrl, TrackingUrlDto>();
        }
    }
}
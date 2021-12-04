using AutoMapper;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Dtos.Tracking;
using NewsLy.Api.Models;

namespace NewsLy.Api.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TrackingUrl, TrackingUrlDto>();
            CreateMap<TrackingUrlCreateDto, TrackingUrl>();
            CreateMap<MailingCreateDto, ContactRequest>();
            CreateMap<MailingList, MailingListDto>();
            CreateMap<ContactRequest, MailRequestDto>();
        }
    }
}
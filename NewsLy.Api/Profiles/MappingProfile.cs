using AutoMapper;
using NewsLy.Api.Dtos.Mailing;
using NewsLy.Api.Dtos.Recipient;
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
            CreateMap<MailingCreateDto, MailRequest>();
            CreateMap<MailingList, MailingListDto>();
            CreateMap<MailRequest, MailRequestDto>();
            CreateMap<RecipientCreateDto, Recipient>();
        }
    }
}
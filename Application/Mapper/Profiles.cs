
using Application.Dto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            // NotificationLog <-> NotificationLogDto
            CreateMap<Notification, NotificationLogDto>()
                .ForMember(dest => dest.ChannelType, opt => opt.MapFrom(src => src.ChannelType.ToString()))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt.ToString("O")));


            // TransactionDto <-> ExternalTransaction
            CreateMap<ExternalTransaction, ExternalTransactionDto>()
                .ReverseMap();
        }
    }
}

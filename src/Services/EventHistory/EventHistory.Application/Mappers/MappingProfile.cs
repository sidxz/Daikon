
using AutoMapper;
using Daikon.EventStore.Models;
using EventHistory.Application.Features.Queries.GetEventHistory;

namespace EventHistory.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventHistoryVM, EventModel>().ReverseMap();
        }
    }
}
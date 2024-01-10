
using AutoMapper;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Features.Commands.NewHitAssessment;

namespace HitAssessment.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<HaCreatedEvent, NewHitAssessmentCommand>().ReverseMap();
        }
    }
}
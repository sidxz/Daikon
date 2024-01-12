
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Features.Commands.DeleteHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.DeleteHitAssessment;
using HitAssessment.Application.Features.Commands.NewHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.NewHitAssessment;
using HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.UpdateHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessmentList;

namespace HitAssessment.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /* Commands */
            CreateMap<HaCreatedEvent, NewHitAssessmentCommand>().ReverseMap();
            CreateMap<HaUpdatedEvent, UpdateHitAssessmentCommand>().ReverseMap();
            CreateMap<HaDeletedEvent, DeleteHitAssessmentCommand>().ReverseMap();

            CreateMap<HaCompoundEvolutionAddedEvent, NewHaCompoundEvolutionCommand>().ReverseMap();
            CreateMap<HaCompoundEvolutionUpdatedEvent, UpdateHaCompoundEvolutionCommand>().ReverseMap();
            CreateMap<HaCompoundEvolutionDeletedEvent, DeleteHaCompoundEvolutionCommand>().ReverseMap();


            /* Events */
            CreateMap<Domain.Entities.HitAssessment, HaCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitAssessment, HaUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HitAssessment, HaDeletedEvent>().ReverseMap();

            CreateMap<Domain.Entities.HaCompoundEvolution, HaCompoundEvolutionAddedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HaCompoundEvolution, HaCompoundEvolutionUpdatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.HaCompoundEvolution, HaCompoundEvolutionDeletedEvent>().ReverseMap();

            /* Queries */
            CreateMap<Domain.Entities.HitAssessment, HitAssessmentListVM>().ReverseMap();

            CreateMap<Domain.Entities.HitAssessment, HitAssessmentVM>()
            .ForMember(dest => dest.HAStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.HAStart)))
            .ForMember(dest => dest.HAPredictedStart, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.HAPredictedStart)))
            .ForMember(dest => dest.HADescription, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.HADescription)))
            .ForMember(dest => dest.HAStatus, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.HAStatus)))
            .ForMember(dest => dest.PrimaryOrg, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.PrimaryOrg)))

            .ReverseMap();
        }
    }
}
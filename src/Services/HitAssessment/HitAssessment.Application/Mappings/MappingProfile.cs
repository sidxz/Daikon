
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
            .ForMember(dest => dest.HaStartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.HaStartDate)))
            .ForMember(dest => dest.HaPredictedStartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.HaPredictedStartDate)))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.Description)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.Status)))
            .ForMember(dest => dest.PrimaryOrgId, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<Guid>, Guid>(src => src.PrimaryOrgId)))

            .ReverseMap();
        }
    }
}
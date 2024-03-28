
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.DTOs.MLogixAPI;
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

            CreateMap<Domain.Entities.HaCompoundEvolution, HaCompoundEvolutionVM>()
            .ForMember(dest => dest.EvolutionDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<DateTime>, DateTime>(src => src.EvolutionDate)))
            .ForMember(dest => dest.Stage, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.Stage)))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.Notes)))
            .ForMember(dest => dest.MIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.MIC)))
            .ForMember(dest => dest.MICUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.MICUnit)))
            .ForMember(dest => dest.IC50, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.IC50)))
            .ForMember(dest => dest.IC50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.IC50Unit)))
            .ForMember(dest => dest.RequestedSMILES, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<string>, string>(src => src.RequestedSMILES)))
            .ForMember(dest => dest.IsStructureDisclosed, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HaCompoundEvolution, IValueProperty<bool>, bool>(src => src.IsStructureDisclosed)))
            .ReverseMap();

            CreateMap<GetMoleculesResultDTO, MoleculeVM>().ReverseMap();
        }
    }
}
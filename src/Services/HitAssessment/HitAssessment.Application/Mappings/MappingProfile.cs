
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.DTOs.MLogixAPI;
using HitAssessment.Application.Features.Batch.ImportOne;
using HitAssessment.Application.Features.Commands.DeleteHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.DeleteHitAssessment;
using HitAssessment.Application.Features.Commands.NewHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.NewHitAssessment;
using HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.UpdateHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessmentList;
using HitAssessment.Domain.Entities;

namespace HitAssessment.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            /* ====== Ha Core ====== */
            // -- Commands --
            CreateMap<Domain.Entities.HitAssessment, Domain.Entities.HitAssessment>();

            CreateMap<HaCreatedEvent, NewHitAssessmentCommand>().ReverseMap();
            CreateMap<HaUpdatedEvent, UpdateHitAssessmentCommand>().ReverseMap();
            CreateMap<HaDeletedEvent, DeleteHitAssessmentCommand>().ReverseMap();


            CreateMap<HaCreatedEvent, Domain.Entities.HitAssessment>().ReverseMap();
            CreateMap<HaUpdatedEvent, Domain.Entities.HitAssessment>().ReverseMap();
            CreateMap<HaDeletedEvent, Domain.Entities.HitAssessment>().ReverseMap();


            // -- Batch --
            CreateMap<ImportOneCommand, HaCreatedEvent>().ReverseMap();

            // -- Queries --

            CreateMap<Domain.Entities.HitAssessment, HitAssessmentListVM>().ReverseMap();

            CreateMap<Domain.Entities.HitAssessment, HitAssessmentVM>()
            .ForMember(dest => dest.HaStartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.HaStartDate)))
            .ForMember(dest => dest.HaPredictedStartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.HaPredictedStartDate)))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.Description)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<string>, string>(src => src.Status)))
            .ForMember(dest => dest.PrimaryOrgId, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<Guid>, Guid>(src => src.PrimaryOrgId)))
            .ForMember(dest => dest.StatusLastModifiedDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusLastModifiedDate)))
            .ForMember(dest => dest.StatusReadyForHADate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusReadyForHADate)))
            .ForMember(dest => dest.StatusActiveDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusActiveDate)))
            .ForMember(dest => dest.StatusIncorrectMzDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusIncorrectMzDate)))
            .ForMember(dest => dest.StatusKnownLiabilityDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusKnownLiabilityDate)))
            .ForMember(dest => dest.StatusCompleteFailedDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusCompleteFailedDate)))
            .ForMember(dest => dest.StatusCompleteSuccessDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.StatusCompleteSuccessDate)))
            .ForMember(dest => dest.RemovalDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.RemovalDate)))
            .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.CompletionDate)))
            .ForMember(dest => dest.EOLDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.EOLDate)))
            .ForMember(dest => dest.H2LPredictedStartDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.HitAssessment, IValueProperty<DateTime>, DateTime>(src => src.H2LPredictedStartDate)))
            .ReverseMap();



            /* ====== Ha Compound Evolution ====== */
            // -- Commands --
            CreateMap<HaCompoundEvolution, HaCompoundEvolution>();

            CreateMap<HaCompoundEvolutionAddedEvent, NewHaCompoundEvolutionCommand>().ReverseMap();
            CreateMap<HaCompoundEvolutionUpdatedEvent, UpdateHaCompoundEvolutionCommand>().ReverseMap();
            CreateMap<HaCompoundEvolutionDeletedEvent, DeleteHaCompoundEvolutionCommand>().ReverseMap();

            CreateMap<HaCompoundEvolutionAddedEvent, HaCompoundEvolution>().ReverseMap();
            CreateMap<HaCompoundEvolutionUpdatedEvent, HaCompoundEvolution>().ReverseMap();
            CreateMap<HaCompoundEvolutionDeletedEvent, HaCompoundEvolution>().ReverseMap();

            // -- Queries --

            CreateMap<HaCompoundEvolution, HaCompoundEvolutionVM>()
            .ForMember(dest => dest.EvolutionDate, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<DateTime>, DateTime>(src => src.EvolutionDate)))
            .ForMember(dest => dest.Stage, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.Stage)))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.Notes)))
            .ForMember(dest => dest.MIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.MIC)))
            .ForMember(dest => dest.MICUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.MICUnit)))
            .ForMember(dest => dest.IC50, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.IC50)))
            .ForMember(dest => dest.IC50Unit, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.IC50Unit)))
            .ForMember(dest => dest.RequestedSMILES, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<string>, string>(src => src.RequestedSMILES)))
            .ForMember(dest => dest.IsStructureDisclosed, opt => opt.MapFrom(new MapperDVariableMetaResolver<HaCompoundEvolution, IValueProperty<bool>, bool>(src => src.IsStructureDisclosed)))
            .ReverseMap();


            CreateMap<RegisterMoleculeResponseDTO, MoleculeVM>().ReverseMap();
            CreateMap<GetMoleculesResultDTO, MoleculeVM>().ReverseMap();
        }
    }
}
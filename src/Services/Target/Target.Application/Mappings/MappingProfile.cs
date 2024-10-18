

using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Targets;
using Target.Application.BatchOperations.BatchCommands.ImportOne;
using Target.Application.Features.Command.DeleteTarget;
using Target.Application.Features.Command.NewTarget;
using Target.Application.Features.Command.UpdateTarget;
using Target.Application.Features.Command.UpdateTargetAssociatedGenes;
using Target.Application.Features.Commands.AddOrUpdateToxicology;
using Target.Application.Features.Commands.AddToxicology;
using Target.Application.Features.Commands.ApproveTarget;
using Target.Application.Features.Commands.DeleteToxicology;
using Target.Application.Features.Commands.RenameTarget;
using Target.Application.Features.Commands.SubmitTPQ;
using Target.Application.Features.Commands.UpdateToxicology;
using Target.Application.Features.Commands.UpdateTPQ;
using Target.Application.Features.Queries.CommonVMs;
using Target.Application.Features.Queries.GetTarget;
using Target.Application.Features.Queries.GetTargetsList;
using Target.Application.Features.Queries.GetTPQ.VMs;
using Target.Domain.Entities;

namespace Target.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            //=== Target Core ===

            CreateMap<Domain.Entities.Target, Domain.Entities.Target>();

            CreateMap<TargetCreatedEvent, NewTargetCommand>().ReverseMap();
            CreateMap<TargetUpdatedEvent, UpdateTargetCommand>().ReverseMap();
            CreateMap<TargetDeletedEvent, DeleteTargetCommand>().ReverseMap();
            CreateMap<TargetRenamedEvent, RenameTargetCommand>().ReverseMap();

            CreateMap<TargetCreatedEvent, Domain.Entities.Target>().ReverseMap();
            CreateMap<TargetUpdatedEvent, Domain.Entities.Target>().ReverseMap();
            CreateMap<TargetDeletedEvent, Domain.Entities.Target>().ReverseMap();
            CreateMap<TargetRenamedEvent, Domain.Entities.Target>().ReverseMap();

            CreateMap<Domain.Entities.Target, TargetVM>()
            .ForMember(dest => dest.Bucket, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<string>, string>(src => src.Bucket)))
            .ForMember(dest => dest.ImpactScore, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.ImpactScore)))
            .ForMember(dest => dest.ImpactComplete, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.ImpactComplete)))
            .ForMember(dest => dest.LikeScore, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.LikeScore)))
            .ForMember(dest => dest.LikeComplete, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.LikeComplete)))
            .ForMember(dest => dest.ScreeningScore, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.ScreeningScore)))
            .ForMember(dest => dest.ScreeningComplete, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.ScreeningComplete)))
            .ForMember(dest => dest.StructureScore, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.StructureScore)))
            .ForMember(dest => dest.StructureComplete, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.StructureComplete)))
            .ForMember(dest => dest.VulnerabilityRatio, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.VulnerabilityRatio)))
            .ForMember(dest => dest.VulnerabilityRank, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.VulnerabilityRank)))
            .ForMember(dest => dest.HTSFeasibility, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.HTSFeasibility)))
            .ForMember(dest => dest.SBDFeasibility, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.SBDFeasibility)))
            .ForMember(dest => dest.Progressibility, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.Progressibility)))
            .ForMember(dest => dest.Safety, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<double>, double>(src => src.Safety)))
            .ForMember(dest => dest.Background, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<string>, string>(src => src.Background)))
            .ForMember(dest => dest.Enablement, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<string>, string>(src => src.Enablement)))
            .ForMember(dest => dest.Strategy, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<string>, string>(src => src.Strategy)))
            .ForMember(dest => dest.Challenges, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Target, IValueProperty<string>, string>(src => src.Challenges)))
            .ReverseMap();

            CreateMap<Domain.Entities.Target, TargetsListVM>();


            // === Toxicology ===
            CreateMap<Domain.Entities.Toxicology, Domain.Entities.Toxicology>();

            CreateMap<TargetToxicologyAddedEvent, AddToxicologyCommand>().ReverseMap();
            CreateMap<TargetToxicologyUpdatedEvent, UpdateToxicologyCommand>().ReverseMap();
            CreateMap<TargetToxicologyDeletedEvent, DeleteToxicologyCommand>().ReverseMap();

            CreateMap<TargetToxicologyAddedEvent, Domain.Entities.Toxicology>().ReverseMap();
            CreateMap<TargetToxicologyUpdatedEvent, Domain.Entities.Toxicology>().ReverseMap();
            CreateMap<TargetToxicologyDeletedEvent, Domain.Entities.Toxicology>().ReverseMap();

            CreateMap<AddOrUpdateToxicologyCommand, AddToxicologyCommand>().ReverseMap();
            CreateMap<AddOrUpdateToxicologyCommand, UpdateToxicologyCommand>().ReverseMap();


            CreateMap<Domain.Entities.Toxicology, ToxicologyVM>()
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Toxicology, IValueProperty<string>, string>(src => src.Topic)))
            .ForMember(dest => dest.Impact, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Toxicology, IValueProperty<string>, string>(src => src.Impact)))
            .ForMember(dest => dest.ImpactPriority, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Toxicology, IValueProperty<bool>, bool>(src => src.ImpactPriority)))
            .ForMember(dest => dest.Likelihood, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Toxicology, IValueProperty<string>, string>(src => src.Likelihood)))
            .ForMember(dest => dest.LikelihoodPriority, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Toxicology, IValueProperty<bool>, bool>(src => src.LikelihoodPriority)))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Toxicology, IValueProperty<string>, string>(src => src.Note)))
            .ReverseMap();


            // === Query ===
            CreateMap<Domain.Entities.PQResponse, PQResponseVM>().ReverseMap();


            // ==== OTHERS ====
            // Command to Command
            CreateMap<NewTargetCommand, ApproveTargetCommand>().ReverseMap();
            CreateMap<ImportOneCommand, ApproveTargetCommand>().ReverseMap();
            CreateMap<ImportOneCommand, SubmitTPQCommand>().ReverseMap();

            // Event to Command

            CreateMap<TargetAssociatedGenesUpdatedEvent, UpdateTargetAssociatedGenesCommand>().ReverseMap();


            CreateMap<TargetPromotionQuestionnaireSubmittedEvent, SubmitTPQCommand>().ReverseMap();
            CreateMap<TargetPromotionQuestionnaireUpdatedEvent, UpdateTPQCommand>().ReverseMap();

            // Command to Domain
            CreateMap<NewTargetCommand, Domain.Entities.Target>().ReverseMap();
            CreateMap<UpdateTargetCommand, Domain.Entities.Target>().ReverseMap();
            CreateMap<UpdateTargetAssociatedGenesCommand, Domain.Entities.Target>().ReverseMap();
            CreateMap<RenameTargetCommand, Domain.Entities.Target>().ReverseMap();

            CreateMap<SubmitTPQCommand, Domain.Entities.PQResponse>().ReverseMap();
            CreateMap<UpdateTPQCommand, Domain.Entities.PQResponse>().ReverseMap();
            CreateMap<PQResponse, PQResponse>();

            // Event to Domain


            CreateMap<TargetPromotionQuestionnaireSubmittedEvent, Domain.Entities.PQResponse>().ReverseMap();
            CreateMap<TargetPromotionQuestionnaireUpdatedEvent, Domain.Entities.PQResponse>().ReverseMap();
            CreateMap<TargetPromotionQuestionnaireDeletedEvent, Domain.Entities.PQResponse>().ReverseMap();


        }
    }
}
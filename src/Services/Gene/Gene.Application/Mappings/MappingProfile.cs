
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Gene;
using Daikon.Events.Strains;
using Gene.Application.BatchOperations.BatchCommands.BatchImportOne;
using Gene.Application.Features.Command.AddExpansionProp;
using Gene.Application.Features.Command.DeleteCrispriStrain;
using Gene.Application.Features.Command.DeleteEssentiality;
using Gene.Application.Features.Command.DeleteExpansionProp;
using Gene.Application.Features.Command.DeleteGene;
using Gene.Application.Features.Command.DeleteHypomorph;
using Gene.Application.Features.Command.DeleteProteinActivityAssay;
using Gene.Application.Features.Command.DeleteProteinProduction;
using Gene.Application.Features.Command.DeleteResistanceMutation;
using Gene.Application.Features.Command.DeleteStrain;
using Gene.Application.Features.Command.DeleteUnpublishedStructuralInformation;
using Gene.Application.Features.Command.DeleteVulnerability;
using Gene.Application.Features.Command.NewCrispriStrain;
using Gene.Application.Features.Command.NewEssentiality;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.NewHypomorph;
using Gene.Application.Features.Command.NewProteinActivityAssay;
using Gene.Application.Features.Command.NewProteinProduction;
using Gene.Application.Features.Command.NewResistanceMutation;
using Gene.Application.Features.Command.NewStrain;
using Gene.Application.Features.Command.NewUnpublishedStructuralInformation;
using Gene.Application.Features.Command.NewVulnerability;
using Gene.Application.Features.Command.UpdateCrispriStrain;
using Gene.Application.Features.Command.UpdateEssentiality;
using Gene.Application.Features.Command.UpdateExpansionProp;
using Gene.Application.Features.Command.UpdateGene;
using Gene.Application.Features.Command.UpdateHypomorph;
using Gene.Application.Features.Command.UpdateProteinActivityAssay;
using Gene.Application.Features.Command.UpdateProteinProduction;
using Gene.Application.Features.Command.UpdateResistanceMutation;
using Gene.Application.Features.Command.UpdateStrain;
using Gene.Application.Features.Command.UpdateUnpublishedStructuralInformation;
using Gene.Application.Features.Command.UpdateVulnerability;
using Gene.Application.Features.Queries.GetGene;
using Gene.Application.Features.Queries.GetGenesList;
using Gene.Application.Features.Queries.GetStrain;
using Gene.Application.Features.Queries.GetStrainsList;
using Gene.Domain.Entities;


namespace Gene.Application.Mappings
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            /* ====== Gene Core ====== */
            // -- Commands --
            CreateMap<Domain.Entities.Gene, Domain.Entities.Gene>();

            CreateMap<GeneCreatedEvent, NewGeneCommand>().ReverseMap();
            CreateMap<GeneUpdatedEvent, UpdateGeneCommand>().ReverseMap();
            CreateMap<GeneDeletedEvent, DeleteGeneCommand>().ReverseMap();

            CreateMap<GeneCreatedEvent, Domain.Entities.Gene>().ReverseMap();
            CreateMap<GeneUpdatedEvent, Domain.Entities.Gene>().ReverseMap();
            CreateMap<GeneDeletedEvent, Domain.Entities.Gene>().ReverseMap();

            // -- Batch --
            CreateMap<BatchImportOneCommand, NewGeneCommand>().ReverseMap();
            CreateMap<BatchImportOneCommand, UpdateGeneCommand>().ReverseMap();


            // -- Queries --
            CreateMap<Domain.Entities.Gene, GenesListVM>().ReverseMap();

            CreateMap<Domain.Entities.Gene, GeneVM>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.Product)))
                .ForMember(dest => dest.FunctionalCategory, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.FunctionalCategory)))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.Comments)))
                .ForMember(dest => dest.GeneSequence, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.GeneSequence)))
                .ForMember(dest => dest.ProteinSequence, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.ProteinSequence)));


             /* ====== Strains ====== */
            // -- Commands --
            CreateMap<Strain, Strain>();

            CreateMap<StrainCreatedEvent, NewStrainCommand>().ReverseMap();
            CreateMap<StrainUpdatedEvent, UpdateStrainCommand>().ReverseMap();
            CreateMap<StrainDeletedEvent, DeleteStrainCommand>().ReverseMap();

            CreateMap<GeneCreatedEvent, Strain>().ReverseMap();
            CreateMap<GeneUpdatedEvent, Strain>().ReverseMap();
            CreateMap<GeneDeletedEvent, Strain>().ReverseMap();

            // -- Queries --
            CreateMap<Strain, StrainsListVM>().ReverseMap();
            CreateMap<Strain, StrainVM>().ReverseMap();


            /* ====== Expansion Prop ====== */
            // -- Commands --
            CreateMap<GeneExpansionProp, GeneExpansionProp>();

            CreateMap<GeneExpansionPropAddedEvent, AddExpansionPropCommand>().ReverseMap();
            CreateMap<GeneExpansionPropUpdatedEvent, UpdateExpansionPropCommand>().ReverseMap();
            CreateMap<GeneExpansionPropDeletedEvent, DeleteExpansionPropCommand>().ReverseMap();

            CreateMap<GeneExpansionPropAddedEvent, GeneExpansionProp>().ReverseMap();
            CreateMap<GeneExpansionPropUpdatedEvent, GeneExpansionProp>().ReverseMap();
            CreateMap<GeneExpansionPropDeletedEvent, GeneExpansionProp>().ReverseMap();

            // -- Batch --
            CreateMap<AddExpansionPropCommand, GeneExpansionProp>().ReverseMap();
            CreateMap<UpdateExpansionPropCommand, GeneExpansionProp>().ReverseMap();

            // -- Queries --
            CreateMap<GeneExpansionProp, ExpansionPropVM>()
                .ForMember(dest => dest.ExpansionValue, opt => opt.MapFrom(new MapperDVariableMetaResolver<GeneExpansionProp, IValueProperty<string>, string>(src => src.ExpansionValue)))
                .ReverseMap();


            /* ====== CRISPRI ====== */
            // -- Commands --
            CreateMap<CrispriStrain, CrispriStrain>();

            CreateMap<GeneCrispriStrainAddedEvent, NewCrispriStrainCommand>().ReverseMap();
            CreateMap<GeneCrispriStrainUpdatedEvent, UpdateCrispriStrainCommand>().ReverseMap();
            CreateMap<GeneCrispriStrainDeletedEvent, DeleteCrispriStrainCommand>().ReverseMap();

            CreateMap<GeneCrispriStrainAddedEvent, CrispriStrain>().ReverseMap();
            CreateMap<GeneCrispriStrainUpdatedEvent, CrispriStrain>().ReverseMap();
            CreateMap<GeneCrispriStrainDeletedEvent, CrispriStrain>().ReverseMap();

            // -- Queries --
            CreateMap<CrispriStrain, GeneCrispriStrainVM>()
                .ForMember(dest => dest.CrispriStrainName, opt => opt.MapFrom(new MapperDVariableMetaResolver<CrispriStrain, IValueProperty<string>, string>(src => src.CrispriStrainName)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<CrispriStrain, IValueProperty<string>, string>(src => src.Notes)));


            /* ====== Essentiality ====== */
            // -- Commands --
            CreateMap<Essentiality, Essentiality>();

            CreateMap<GeneEssentialityAddedEvent, NewEssentialityCommand>().ReverseMap();
            CreateMap<GeneEssentialityUpdatedEvent, UpdateEssentialityCommand>().ReverseMap();
            CreateMap<GeneEssentialityDeletedEvent, DeleteEssentialityCommand>().ReverseMap();

            CreateMap<GeneEssentialityAddedEvent, Essentiality>().ReverseMap();
            CreateMap<GeneEssentialityUpdatedEvent, Essentiality>().ReverseMap();
            CreateMap<GeneEssentialityDeletedEvent, Essentiality>().ReverseMap();

            // -- Queries --
            CreateMap<Essentiality, GeneEssentialityVM>()
                .ForMember(dest => dest.Classification, opt => opt.MapFrom(new MapperDVariableMetaResolver<Essentiality, IValueProperty<string>, string>(src => src.Classification)))
                .ForMember(dest => dest.Condition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Essentiality, IValueProperty<string>, string>(src => src.Condition)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Essentiality, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Essentiality, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(new MapperDVariableMetaResolver<Essentiality, IValueProperty<string>, string>(src => src.Note)));


            /* ====== Hypomorph ====== */
            // -- Commands --
            CreateMap<Hypomorph, Hypomorph>();

            CreateMap<GeneHypomorphAddedEvent, NewHypomorphCommand>().ReverseMap();
            CreateMap<GeneHypomorphUpdatedEvent, UpdateHypomorphCommand>().ReverseMap();
            CreateMap<GeneHypomorphDeletedEvent, DeleteHypomorphCommand>().ReverseMap();

            CreateMap<GeneHypomorphAddedEvent, Hypomorph>().ReverseMap();
            CreateMap<GeneHypomorphUpdatedEvent, Hypomorph>().ReverseMap();
            CreateMap<GeneHypomorphDeletedEvent, Hypomorph>().ReverseMap();

            // -- Queries --
            CreateMap<Hypomorph, GeneHypomorphVM>()
                .ForMember(dest => dest.KnockdownStrain, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.KnockdownStrain)))
                .ForMember(dest => dest.Phenotype, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.Phenotype)))
                .ForMember(dest => dest.GrowthDefect, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.GrowthDefect)))
                .ForMember(dest => dest.GrowthDefectSeverity, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.GrowthDefectSeverity)))
                .ForMember(dest => dest.KnockdownLevel, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.KnockdownLevel)))
                .ForMember(dest => dest.EstimatedKnockdownRelativeToWT, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.EstimatedKnockdownRelativeToWT)))
                .ForMember(dest => dest.EstimateBasedOn, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.EstimateBasedOn)))
                .ForMember(dest => dest.SelectivelySensitizesToOnTargetInhibitors, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.SelectivelySensitizesToOnTargetInhibitors)))
                .ForMember(dest => dest.SuitableForScreening, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.SuitableForScreening)))
                .ForMember(dest => dest.Researcher, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.Researcher)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.URL)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Hypomorph, IValueProperty<string>, string>(src => src.Notes)));

            /* ====== ProteinActivityAssay ====== */
            // -- Commands --
            CreateMap<ProteinActivityAssay, ProteinActivityAssay>();

            CreateMap<GeneProteinActivityAssayAddedEvent, NewProteinActivityAssayCommand>().ReverseMap();
            CreateMap<GeneProteinActivityAssayUpdatedEvent, UpdateProteinActivityAssayCommand>().ReverseMap();
            CreateMap<GeneProteinActivityAssayDeletedEvent, DeleteProteinActivityAssayCommand>().ReverseMap();

            CreateMap<GeneProteinActivityAssayAddedEvent, ProteinActivityAssay>().ReverseMap();
            CreateMap<GeneProteinActivityAssayUpdatedEvent, ProteinActivityAssay>().ReverseMap();
            CreateMap<GeneProteinActivityAssayDeletedEvent, ProteinActivityAssay>().ReverseMap();

            // -- Queries --
            CreateMap<ProteinActivityAssay, GeneProteinActivityAssayVM>()
            .ForMember(dest => dest.Assay, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinActivityAssay, IValueProperty<string>, string>(src => src.Assay)))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinActivityAssay, IValueProperty<string>, string>(src => src.Method)))
            .ForMember(dest => dest.Throughput, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinActivityAssay, IValueProperty<string>, string>(src => src.Throughput)))
            .ForMember(dest => dest.PMID, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinActivityAssay, IValueProperty<string>, string>(src => src.PMID)))
            .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinActivityAssay, IValueProperty<string>, string>(src => src.Reference)))
            .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinActivityAssay, IValueProperty<string>, string>(src => src.URL)));


            /* ====== ProteinProduction ====== */
            // -- Commands --
            CreateMap<ProteinProduction, ProteinProduction>();

            CreateMap<GeneProteinProductionAddedEvent, NewProteinProductionCommand>().ReverseMap();
            CreateMap<GeneProteinProductionUpdatedEvent, UpdateProteinProductionCommand>().ReverseMap();
            CreateMap<GeneProteinProductionDeletedEvent, DeleteProteinProductionCommand>().ReverseMap();

            CreateMap<GeneProteinProductionAddedEvent, ProteinProduction>().ReverseMap();
            CreateMap<GeneProteinProductionUpdatedEvent, ProteinProduction>().ReverseMap();
            CreateMap<GeneProteinProductionDeletedEvent, ProteinProduction>().ReverseMap();

            // -- Queries --
            CreateMap<ProteinProduction, GeneProteinProductionVM>()
                .ForMember(dest => dest.Production, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<string>, string>(src => src.Production)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Purity, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<string>, string>(src => src.Purity)))
                .ForMember(dest => dest.DateProduced, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<DateTime>, DateTime>(src => src.DateProduced)))
                .ForMember(dest => dest.PMID, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<string>, string>(src => src.PMID)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<ProteinProduction, IValueProperty<string>, string>(src => src.URL)));



            /* ====== ResistanceMutation ====== */
            // -- Commands --
            CreateMap<ResistanceMutation, ResistanceMutation>();

            CreateMap<GeneResistanceMutationAddedEvent, NewResistanceMutationCommand>().ReverseMap();
            CreateMap<GeneResistanceMutationUpdatedEvent, UpdateResistanceMutationCommand>().ReverseMap();
            CreateMap<GeneResistanceMutationDeletedEvent, DeleteResistanceMutationCommand>().ReverseMap();

            CreateMap<GeneResistanceMutationAddedEvent, ResistanceMutation>().ReverseMap();
            CreateMap<GeneResistanceMutationUpdatedEvent, ResistanceMutation>().ReverseMap();
            CreateMap<GeneResistanceMutationDeletedEvent, ResistanceMutation>().ReverseMap();

            // -- Queries --
            CreateMap<ResistanceMutation, GeneResistanceMutationVM>()
                .ForMember(dest => dest.Mutation, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Mutation)))
                .ForMember(dest => dest.Isolate, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Isolate)))
                .ForMember(dest => dest.ParentStrain, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.ParentStrain)))
                .ForMember(dest => dest.Compound, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Compound)))
                .ForMember(dest => dest.ShiftInMIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.ShiftInMIC)))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Organization)))
                .ForMember(dest => dest.Researcher, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Researcher)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<ResistanceMutation, IValueProperty<string>, string>(src => src.Notes)));


            /* ====== UnpublishedStructuralInformation ====== */
            // -- Commands --
            CreateMap<UnpublishedStructuralInformation, UnpublishedStructuralInformation>();

            CreateMap<GeneUnpublishedStructuralInformationAddedEvent, NewUnpublishedStructuralInformationCommand>().ReverseMap();
            CreateMap<GeneUnpublishedStructuralInformationUpdatedEvent, UpdateUnpublishedStructuralInformationCommand>().ReverseMap();
            CreateMap<GeneUnpublishedStructuralInformationDeletedEvent, DeleteUnpublishedStructuralInformationCommand>().ReverseMap();

            CreateMap<GeneUnpublishedStructuralInformationAddedEvent, UnpublishedStructuralInformation>().ReverseMap();
            CreateMap<GeneUnpublishedStructuralInformationUpdatedEvent, UnpublishedStructuralInformation>().ReverseMap();
            CreateMap<GeneUnpublishedStructuralInformationDeletedEvent, UnpublishedStructuralInformation>().ReverseMap();

            // -- Queries --
             CreateMap<UnpublishedStructuralInformation, GeneUnpublishedStructuralInformationVM>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Organization)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Resolution, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Resolution)))
                .ForMember(dest => dest.ResolutionUnit, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.ResolutionUnit)))
                .ForMember(dest => dest.Ligands, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Ligands)))
                .ForMember(dest => dest.Researcher, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Researcher)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.URL)));



            /* ====== Vulnerability ====== */
            // -- Commands --
            CreateMap<Vulnerability, Vulnerability>();

            CreateMap<GeneVulnerabilityAddedEvent, NewVulnerabilityCommand>().ReverseMap();
            CreateMap<GeneVulnerabilityUpdatedEvent, UpdateVulnerabilityCommand>().ReverseMap();
            CreateMap<GeneVulnerabilityDeletedEvent, DeleteVulnerabilityCommand>().ReverseMap();

            CreateMap<GeneVulnerabilityAddedEvent, Vulnerability>().ReverseMap();
            CreateMap<GeneVulnerabilityUpdatedEvent, Vulnerability>().ReverseMap();
            CreateMap<GeneVulnerabilityDeletedEvent, Vulnerability>().ReverseMap();

            // -- Queries --

            CreateMap<Vulnerability, GeneVulnerabilityVM>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.Rank)))
                .ForMember(dest => dest.ViUpperBound, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.ViUpperBound)))
                .ForMember(dest => dest.ViLowerBound, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.ViLowerBound)))
                .ForMember(dest => dest.VulnerabilityIndex, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.VulnerabilityIndex)))
                .ForMember(dest => dest.VulnerabilityCondition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.VulnerabilityCondition)))
                .ForMember(dest => dest.TnSeqEss, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.TnSeqEss)))
                .ForMember(dest => dest.CrisprEss, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.CrisprEss)))
                .ForMember(dest => dest.HighConfidenceVulnerabilityCall, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.HighConfidenceVulnerabilityCall)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<Vulnerability, IValueProperty<string>, string>(src => src.URL)));
        }
    }
}
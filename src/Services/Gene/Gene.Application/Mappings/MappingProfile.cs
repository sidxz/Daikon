
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Gene;
using Gene.Application.Features.Command.AddExpansionProp;
using Gene.Application.Features.Command.DeleteExpansionProp;
using Gene.Application.Features.Command.DeleteGene;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.UpdateExpansionProp;
using Gene.Application.Features.Command.UpdateGene;
using Gene.Application.Features.Queries.GetGene;
using Gene.Application.Features.Queries.GetGenesList;
using Gene.Domain.Entities;


namespace Gene.Application.Mappings
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

           
            CreateMap<Domain.Entities.Essentiality, Features.Queries.GetGene.GeneEssentialityVM>()
                .ForMember(dest => dest.Classification, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Essentiality, IValueProperty<string>, string>(src => src.Classification)))
                .ForMember(dest => dest.Condition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Essentiality, IValueProperty<string>, string>(src => src.Condition)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Essentiality, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Essentiality, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Essentiality, IValueProperty<string>, string>(src => src.Note)));

            CreateMap<Domain.Entities.ProteinProduction, Features.Queries.GetGene.GeneProteinProductionVM>()
                .ForMember(dest => dest.Production, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<string>, string>(src => src.Production)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Purity, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<string>, string>(src => src.Purity)))
                .ForMember(dest => dest.DateProduced, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<DateTime>, DateTime>(src => src.DateProduced)))
                .ForMember(dest => dest.PMID, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<string>, string>(src => src.PMID)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinProduction, IValueProperty<string>, string>(src => src.URL)));

            CreateMap<Domain.Entities.ProteinActivityAssay, Features.Queries.GetGene.GeneProteinActivityAssayVM>()
                .ForMember(dest => dest.Assay, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinActivityAssay, IValueProperty<string>, string>(src => src.Assay)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinActivityAssay, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Throughput, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinActivityAssay, IValueProperty<string>, string>(src => src.Throughput)))
                .ForMember(dest => dest.PMID, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinActivityAssay, IValueProperty<string>, string>(src => src.PMID)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinActivityAssay, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ProteinActivityAssay, IValueProperty<string>, string>(src => src.URL)));

            CreateMap<Domain.Entities.Hypomorph, Features.Queries.GetGene.GeneHypomorphVM>()
                .ForMember(dest => dest.KnockdownStrain, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hypomorph, IValueProperty<string>, string>(src => src.KnockdownStrain)))
                .ForMember(dest => dest.Phenotype, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hypomorph, IValueProperty<string>, string>(src => src.Phenotype)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Hypomorph, IValueProperty<string>, string>(src => src.Notes)));

            CreateMap<Domain.Entities.CrispriStrain, Features.Queries.GetGene.GeneCrispriStrainVM>()
                .ForMember(dest => dest.CrispriStrainName, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.CrispriStrain, IValueProperty<string>, string>(src => src.CrispriStrainName)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.CrispriStrain, IValueProperty<string>, string>(src => src.Notes)));


            CreateMap<Domain.Entities.ResistanceMutation, Features.Queries.GetGene.GeneResistanceMutationVM>()
                .ForMember(dest => dest.Mutation, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Mutation)))
                .ForMember(dest => dest.Isolate, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Isolate)))
                .ForMember(dest => dest.ParentStrain, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.ParentStrain)))
                .ForMember(dest => dest.Compound, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Compound)))
                .ForMember(dest => dest.ShiftInMIC, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.ShiftInMIC)))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Organization)))
                .ForMember(dest => dest.Researcher, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Researcher)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.ResistanceMutation, IValueProperty<string>, string>(src => src.Notes)));


            CreateMap<Domain.Entities.Vulnerability, Features.Queries.GetGene.GeneVulnerabilityVM>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.Rank)))
                .ForMember(dest => dest.ViUpperBound, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.ViUpperBound)))
                .ForMember(dest => dest.ViLowerBound, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.ViLowerBound)))
                .ForMember(dest => dest.VulnerabilityIndex, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.VulnerabilityIndex)))
                .ForMember(dest => dest.VulnerabilityCondition, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.VulnerabilityCondition)))
                .ForMember(dest => dest.TnSeqEss, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.TnSeqEss)))
                .ForMember(dest => dest.CrisprEss, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.CrisprEss)))
                .ForMember(dest => dest.HighConfidenceVulnerabilityCall, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.HighConfidenceVulnerabilityCall)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Vulnerability, IValueProperty<string>, string>(src => src.URL)));

            CreateMap<Domain.Entities.UnpublishedStructuralInformation, Features.Queries.GetGene.GeneUnpublishedStructuralInformationVM>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Organization)))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Method)))
                .ForMember(dest => dest.Resolution, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Resolution)))
                .ForMember(dest => dest.Ligands, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Ligands)))
                .ForMember(dest => dest.Researcher, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Researcher)))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Reference)))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.Notes)))
                .ForMember(dest => dest.URL, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.UnpublishedStructuralInformation, IValueProperty<string>, string>(src => src.URL)));



            /* ====== Gene Core ====== */
            // -- Commands --
            CreateMap<Domain.Entities.Gene, Domain.Entities.Gene>();

            CreateMap<GeneCreatedEvent, NewGeneCommand>().ReverseMap();
            CreateMap<GeneUpdatedEvent, UpdateGeneCommand>().ReverseMap();
            CreateMap<GeneDeletedEvent, DeleteGeneCommand>().ReverseMap();

            CreateMap<GeneCreatedEvent, Domain.Entities.Gene>().ReverseMap();
            CreateMap<GeneUpdatedEvent, Domain.Entities.Gene>().ReverseMap();
            CreateMap<GeneDeletedEvent, Domain.Entities.Gene>().ReverseMap();

            // -- Queries --
            CreateMap<Domain.Entities.Gene, GenesListVM>().ReverseMap();

            CreateMap<Domain.Entities.Gene, GeneVM>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.Product)))
                .ForMember(dest => dest.FunctionalCategory, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.FunctionalCategory)))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.Comments)))
                .ForMember(dest => dest.GeneSequence, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.GeneSequence)))
                .ForMember(dest => dest.ProteinSequence, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.ProteinSequence)));


            /* ====== Expansion Prop ====== */
            // -- Commands --
            CreateMap<GeneExpansionProp, GeneExpansionProp>();

            CreateMap<GeneExpansionPropAddedEvent, AddExpansionPropCommand>().ReverseMap();
            CreateMap<GeneExpansionPropUpdatedEvent, UpdateExpansionPropCommand>().ReverseMap();
            CreateMap<GeneExpansionPropDeletedEvent, DeleteExpansionPropCommand>().ReverseMap();

            CreateMap<GeneExpansionPropAddedEvent, GeneExpansionProp>().ReverseMap();
            CreateMap<GeneExpansionPropUpdatedEvent, GeneExpansionProp>().ReverseMap();
            CreateMap<GeneExpansionPropDeletedEvent, GeneExpansionProp>().ReverseMap();
            
            // -- Queries --
            CreateMap<GeneExpansionProp, ExpansionPropVM>()
                .ForMember(dest => dest.ExpansionValue, opt => opt.MapFrom(new MapperDVariableMetaResolver<GeneExpansionProp, IValueProperty<string>, string>(src => src.ExpansionValue)))
                .ReverseMap();

        }
    }
}
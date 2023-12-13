
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;


namespace Gene.Application.Mappings
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Domain.Entities.Gene, Features.Queries.GetGenesList.GenesListVM>().ReverseMap();

            CreateMap<Domain.Entities.Gene, Features.Queries.GetGene.GeneVM>()
                .ForMember(dest => dest.Function, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.Function)))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.Product)))
                .ForMember(dest => dest.FunctionalCategory, opt => opt.MapFrom(new MapperDVariableMetaResolver<Domain.Entities.Gene, IValueProperty<string>, string>(src => src.FunctionalCategory)));

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

            CreateMap<Features.Command.NewGene.NewGeneCommand , Domain.Entities.Gene>().ReverseMap();
            CreateMap<Features.Command.UpdateGene.UpdateGeneCommand, Domain.Entities.Gene>().ReverseMap();

            CreateMap<Features.Command.NewEssentiality.NewEssentialityCommand, Domain.Entities.Essentiality>().ReverseMap();
            CreateMap<Features.Command.UpdateEssentiality.UpdateEssentialityCommand, Domain.Entities.Essentiality>().ReverseMap();

            CreateMap<Features.Command.NewProteinProduction.NewProteinProductionCommand, Domain.Entities.ProteinProduction>().ReverseMap();
            CreateMap<Features.Command.UpdateProteinProduction.UpdateProteinProductionCommand, Domain.Entities.ProteinProduction>().ReverseMap();

            CreateMap<Features.Command.NewProteinActivityAssay.NewProteinActivityAssayCommand, Domain.Entities.ProteinActivityAssay>().ReverseMap();
            CreateMap<Features.Command.UpdateProteinActivityAssay.UpdateProteinActivityAssayCommand, Domain.Entities.ProteinActivityAssay>().ReverseMap();
            
            CreateMap<Features.Command.NewHypomorph.NewHypomorphCommand, Domain.Entities.Hypomorph>().ReverseMap();
            CreateMap<Features.Command.UpdateHypomorph.UpdateHypomorphCommand, Domain.Entities.Hypomorph>().ReverseMap();

            CreateMap<Features.Command.NewCrispriStrain.NewCrispriStrainCommand, Domain.Entities.CrispriStrain>().ReverseMap();
            CreateMap<Features.Command.UpdateCrispriStrain.UpdateCrispriStrainCommand, Domain.Entities.CrispriStrain>().ReverseMap();

            CreateMap<Features.Command.NewResistanceMutation.NewResistanceMutationCommand, Domain.Entities.ResistanceMutation>().ReverseMap();
            CreateMap<Features.Command.UpdateResistanceMutation.UpdateResistanceMutationCommand, Domain.Entities.ResistanceMutation>().ReverseMap();
        }
    }
}
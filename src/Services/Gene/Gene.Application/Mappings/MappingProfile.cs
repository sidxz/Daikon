
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
                


            CreateMap<Features.Command.NewGene.NewGeneCommand , Domain.Entities.Gene>().ReverseMap();
            CreateMap<Features.Command.UpdateGene.UpdateGeneCommand, Domain.Entities.Gene>().ReverseMap();

            CreateMap<Features.Command.NewEssentiality.NewEssentialityCommand, Domain.Entities.Essentiality>().ReverseMap();
            CreateMap<Features.Command.UpdateEssentiality.UpdateEssentialityCommand, Domain.Entities.Essentiality>().ReverseMap();
            
        }
    }
}
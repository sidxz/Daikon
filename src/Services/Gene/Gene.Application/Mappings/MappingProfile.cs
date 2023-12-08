
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

            CreateMap<Features.Command.NewGene.NewGeneCommand , Domain.Entities.Gene>().ReverseMap();
            CreateMap<Features.Command.UpdateGene.UpdateGeneCommand, Domain.Entities.Gene>().ReverseMap();
        }
    }
}
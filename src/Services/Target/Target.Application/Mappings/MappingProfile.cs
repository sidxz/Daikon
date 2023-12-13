

using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.Targets;
using Target.Application.Features.Command.NewTarget;
using Target.Application.Features.Command.UpdateTarget;
using Target.Application.Features.Queries.GetTarget;

namespace Target.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewTargetCommand, Domain.Entities.Target>().ReverseMap();
            CreateMap<UpdateTargetCommand, Domain.Entities.Target>().ReverseMap();


            CreateMap<Domain.Entities.Target, TargetCreatedEvent>().ReverseMap();
            CreateMap<Domain.Entities.Target, TargetUpdatedEvent>().ReverseMap();


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

            .ReverseMap();


            
        }
    }
}
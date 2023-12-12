

using AutoMapper;
using Daikon.Events.Targets;
using Target.Application.Features.Command.NewTarget;

namespace Target.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewTargetCommand, Domain.Entities.Target>().ReverseMap();

            
            CreateMap<Domain.Entities.Target, TargetCreatedEvent>().ReverseMap();
        }
    }
}
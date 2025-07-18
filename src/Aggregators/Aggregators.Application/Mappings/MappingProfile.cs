
using Aggregators.Application.Disclosure.Dashboard;
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Shared.VM.MLogix;

namespace Aggregators.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<MoleculeVM, DisclosureDashTableElemView>().ReverseMap();

        }
    }
}
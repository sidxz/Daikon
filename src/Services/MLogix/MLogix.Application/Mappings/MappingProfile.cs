
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Resolvers;
using Daikon.Events.MLogix;
using MLogix.Application.DTOs.MolDbAPI;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Commands.UpdateMolecule;
using MLogix.Application.Features.Queries.GetMolecule;
using MLogix.Application.Features.Queries.ListMolecules;
using MLogix.Domain.Entities;

namespace MLogix.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /* Commands */

            CreateMap<MoleculeCreatedEvent, RegisterMoleculeCommand>()
                .ReverseMap();

            CreateMap<MoleculeDTO, MoleculeDTO>().ReverseMap();

            CreateMap<UpdateMoleculeResponseDTO, MoleculeUpdatedEvent>()
                .ReverseMap();


            /* Events */
            CreateMap<Molecule, MoleculeCreatedEvent>().ReverseMap();
            CreateMap<Molecule, MoleculeUpdatedEvent>().ReverseMap();

            /* Queries */
            CreateMap<Molecule, MoleculeVM>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(new MapperDVariableMetaResolver<Molecule, IValueProperty<string>, string>(src => src.Name)))
            .ReverseMap();

            CreateMap<Molecule, MoleculeListVM>()
                .ReverseMap();
        }
    }
}
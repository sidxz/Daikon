
using AutoMapper;
using Daikon.Events.MLogix;
using MLogix.Application.DTOs.MolDbAPI;
using MLogix.Application.Features.Commands.RegisterMolecule;
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

            CreateMap<CompoundDTO, CompoundDTO>().ReverseMap();


            /* Events */
            CreateMap<Molecule, MoleculeCreatedEvent>().ReverseMap();
        }
    }
}
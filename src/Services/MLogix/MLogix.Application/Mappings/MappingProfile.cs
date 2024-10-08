
using AutoMapper;
using Daikon.Events.MLogix;
using Daikon.Shared.VM.MLogix;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using MLogix.Application.Features.Commands.UpdateMolecule;
using MLogix.Application.Features.Queries.FindSimilarMolecules;
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

            CreateMap<UpdateMoleculeResponseDTO, MoleculeUpdatedEvent>()
                .ReverseMap();

            CreateMap<RegisterMoleculeBatchCommand, RegisterMoleculeBatchCommand>()
                .ReverseMap();

            CreateMap<RegisterMoleculeCommandWithRegId, RegisterMoleculeCommandWithRegId>()
                .ReverseMap();

            CreateMap<RegisterMoleculeCommand, RegisterMoleculeCommand>()
                .ReverseMap();


            /* Events */
            CreateMap<MoleculeBase, MoleculeCreatedEvent>().ReverseMap();
            CreateMap<MoleculeBase, MoleculeUpdatedEvent>().ReverseMap();
            CreateMap<Molecule, MoleculeCreatedEvent>().ReverseMap();
            CreateMap<Molecule, MoleculeUpdatedEvent>().ReverseMap();

            /* Queries */
            CreateMap<Molecule, MoleculeVM>()
            .ReverseMap();

            CreateMap<MoleculeBase, MoleculeVM>().ReverseMap();
            CreateMap<Molecule, SimilarMoleculeVM>().ReverseMap();
            CreateMap<SimilarMolecule, SimilarMoleculeVM>().ReverseMap();

            CreateMap<RegisterMoleculeResponseDTO, MoleculeBase>().ReverseMap();
            CreateMap<UpdateMoleculeResponseDTO, MoleculeBase>().ReverseMap();
        }
    }
}
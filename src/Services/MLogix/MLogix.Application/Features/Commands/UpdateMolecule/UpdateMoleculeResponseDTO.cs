using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeResponseDTO : MoleculeBase
    {

        public Guid RegistrationId { get; set; }
        public bool WasAlreadyRegistered { get; set; }

    }
}
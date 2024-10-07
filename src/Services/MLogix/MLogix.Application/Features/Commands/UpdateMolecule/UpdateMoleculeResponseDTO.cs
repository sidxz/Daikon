using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.VM.MLogix;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeResponseDTO : MoleculeVM
    {
        public bool WasAlreadyRegistered { get; set; }

    }
}
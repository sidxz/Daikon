using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;
using MLogix.Application.Features.Commands.RegisterMolecule;

namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    public class RegisterMoleculeCommandWithRegId : RegisterMoleculeCommand
    {
        public Guid RegistrationId { get; set; }
    }
    public class RegisterMoleculeBatchCommand : BaseCommand, IRequest<List<RegisterMoleculeResponseDTO>>
    {
        public List<RegisterMoleculeCommandWithRegId> Commands { get; set; } = [];
    }
}
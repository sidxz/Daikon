using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Project.Application.Features.Commands.UpdateProjectAssociation
{
    public class UpdateProjectAssociationCommand : BaseCommand, IRequest<Unit>
    {
        /* Associated Hit Assessment */
        public Guid? HaId { get; set; } = default!;
        public Guid? CompoundId { get; set; } = default!;
        public string? CompoundSMILES { get; set; } = default!;
        public Guid? HitCompoundId { get; set; } = default!;
        public Guid? HitId { get; set; } = default!;
    }
}
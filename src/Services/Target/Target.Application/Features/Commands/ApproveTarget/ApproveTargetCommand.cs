using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Commands.ApproveTarget
{
    public class ApproveTargetCommand : BaseCommand, IRequest<Unit>
    {
        public Guid TPQId { get; set; }
        public List<Tuple<string, string, string>> Response { get; set; }
        public Guid StrainId { get; set; }
        public string TargetName { get; set; }
        public Dictionary<string, string>? AssociatedGenes { get; set; }
    }
}
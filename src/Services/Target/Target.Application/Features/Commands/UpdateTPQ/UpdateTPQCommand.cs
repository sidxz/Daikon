using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Commands.UpdateTPQ
{
    public class UpdateTPQCommand : BaseCommand, IRequest<Unit>
    {
        public string RequestedTargetName { get; set; }
        public Dictionary<string, string>? RequestedAssociatedGenes { get; set; }
        public Dictionary<string, string>? ApprovedAssociatedGenes { get; set; }
        public Guid StrainId { get; set; }
        public List<Tuple<string, string, string>> Response { get; set; }
    }

}
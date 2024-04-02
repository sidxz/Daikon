
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Commands.SubmitTPQ
{
    public class SubmitTPQCommand : BaseCommand, IRequest<Unit>
    {
        public Guid? RequestedBy { get; set; }
        
        public string RequestedTargetName { get; set; }
        public Dictionary<string, string>? RequestedAssociatedGenes { get; set; }
        public Guid StrainId { get; set; }
        //public List<(string QIdentification, string Answer, string Description)> Response { get; set; }
        public List<Tuple<string, string, string>> Response { get; set; }
    }
}
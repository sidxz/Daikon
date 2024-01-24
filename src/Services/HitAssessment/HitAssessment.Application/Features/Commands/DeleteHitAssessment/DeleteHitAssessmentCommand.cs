
using CQRS.Core.Command;
using MediatR;

namespace HitAssessment.Application.Features.Commands.DeleteHitAssessment
{
    public class DeleteHitAssessmentCommand : BaseCommand, IRequest<Unit>
    {
        public Guid StrainId { get; set; }
        public string Name { get; set; }


    }
}
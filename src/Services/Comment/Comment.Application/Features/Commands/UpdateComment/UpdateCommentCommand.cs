
using CQRS.Core.Command;
using CQRS.Core.Domain;
using MediatR;

namespace Comment.Application.Features.Commands.UpdateComment
{
    public class UpdateCommentCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ResourceId { get; set; }
        public string? Reference { get; set; }
        public string? Section { get; set; }
        public DVariable<string> Topic { get; set; }
        public DVariable<string>? Description { get; set; }
        public string? PostedBy { get; set; }
        
    }
    
}
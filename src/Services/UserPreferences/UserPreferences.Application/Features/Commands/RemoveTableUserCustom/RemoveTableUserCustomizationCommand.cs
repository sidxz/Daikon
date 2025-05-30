
using CQRS.Core.Command;
using MediatR;

namespace UserPreferences.Application.Features.Commands.RemoveTableUserCustom
{
    public class RemoveTableUserCustomizationCommand : BaseCommand, IRequest<Unit>
    {
        public required string TableType { get; set; }
        public required Guid TableInstanceId { get; set; }
    }
}
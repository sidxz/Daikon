
using CQRS.Core.Command;
using MediatR;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Commands.SetTableGlobal
{
    public class SetTableGlobalConfigCommand : BaseCommand, IRequest<TableGlobalConfig>
    {
        public required string TableType { get; set; }
        public required Guid TableInstanceId { get; set; }
        public List<string> Columns { get; set; } = [];
        public DateTime LastModified { get; set; }
        public int Version { get; set; }
    }
}
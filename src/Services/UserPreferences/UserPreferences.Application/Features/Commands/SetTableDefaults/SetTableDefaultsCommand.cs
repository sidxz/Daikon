
using CQRS.Core.Command;
using MediatR;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Commands.SetTableDefaults
{
    public class SetTableDefaultsCommand : BaseCommand, IRequest<TableDefaults>
    {
        public required string TableType { get; set; }  // "hit", "screen", etc
        public List<string> Columns { get; set; } = [];
        public DateTime LastModified { get; set; }
        public int Version { get; set; }
    }
}
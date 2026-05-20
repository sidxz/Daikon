using CQRS.Core.Command;
using MediatR;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Commands.RefreshAdmet
{
    public class RefreshAdmetCommand : BaseCommand, IRequest<AdmetBackfillTriggerVM>
    {
        public int ChunkSize { get; set; } = 1000;
        public int? Limit { get; set; }
        public bool IncludeErrors { get; set; } = false;
    }
}

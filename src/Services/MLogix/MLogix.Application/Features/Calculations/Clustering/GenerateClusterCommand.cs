
using CQRS.Core.Command;
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using MediatR;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Calculations.Clustering
{
    public class GenerateClusterCommand : BaseCommand, IRequest<List<ClusterVM>>
    {
        public List<ClusterDTO> Molecules { get; set; }
    }
}
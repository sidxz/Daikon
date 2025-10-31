using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;
using MLogix.Application.DTOs.CageFusion;

namespace MLogix.Application.Features.Commands.PredictNuisance
{
    public class PredictNuisanceCommand : BaseCommand, IRequest<NuisanceResponseDTO>
    {
        public List<NuisanceRequestTuple> NuisanceRequestTuple { get; set; } = [];
        public bool PlotAllAttention { get; set; } = false;
    }
}
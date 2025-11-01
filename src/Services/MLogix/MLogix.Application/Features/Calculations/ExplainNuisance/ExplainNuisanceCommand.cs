using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;
using MLogix.Application.DTOs.CageFusion;

namespace MLogix.Application.Features.Calculations.ExplainNuisance
{
    public class ExplainNuisanceCommand : BaseCommand, IRequest<NuisanceResponseDTO>
    {
        public NuisanceRequestTuple NuisanceRequestTuple { get; set; } = new NuisanceRequestTuple();
        public bool PlotAllAttention { get; set; } = true;
    }

}
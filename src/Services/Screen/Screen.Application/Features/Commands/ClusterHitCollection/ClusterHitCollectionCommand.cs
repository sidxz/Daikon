using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using Daikon.Shared.VM.Screen;
using MediatR;

namespace Screen.Application.Features.Commands.ClusterHitCollection
{
    public class ClusterHitCollectionCommand : BaseCommand, IRequest<List<HitVM>>
    {
        public double CutOff { get; set; }
    }
}
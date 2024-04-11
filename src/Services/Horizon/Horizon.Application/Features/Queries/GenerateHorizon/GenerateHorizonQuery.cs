using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Horizon.Application.Contracts.Persistance;
using Horizon.Application.VMs.D3;
using MediatR;

namespace Horizon.Application.Features.Queries.GenerateHorizon
{
    public class GenerateHorizonQuery :BaseQuery, IRequest<D3Node>
    {
      public Guid Id { get; set; }
    }
}
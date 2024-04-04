using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Horizon.Application.Contracts.Persistance;
using MediatR;

namespace Horizon.Application.Features.Queries.GenerateHorizon
{
    public class GenerateHorizonQuery :BaseQuery, IRequest<GenerateHorizonResponseVM>
    {
      public Guid Id { get; set; }
    }
}
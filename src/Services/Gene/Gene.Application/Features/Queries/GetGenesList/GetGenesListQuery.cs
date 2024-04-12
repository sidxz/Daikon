using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Gene.Application.Features.Queries.GetGenesList
{
    public class GetGenesListQuery : BaseQuery, IRequest<List<GenesListVM>>
    {
        
    }
}
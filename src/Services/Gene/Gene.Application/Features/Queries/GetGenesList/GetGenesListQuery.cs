using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Gene.Application.Features.Queries.GetGenesList
{
    public class GetGenesListQuery : IRequest<List<GenesListVM>>
    {
        
    }
}
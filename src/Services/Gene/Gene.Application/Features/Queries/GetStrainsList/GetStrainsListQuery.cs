using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Gene.Application.Features.Queries.GetStrainsList
{
    public class GetStrainsListQuery : IRequest<List<StrainsListVM>>
    {
        
    }
}
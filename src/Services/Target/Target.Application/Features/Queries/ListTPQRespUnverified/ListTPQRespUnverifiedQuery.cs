using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.ListTPQRespUnverified
{
    public class ListTPQRespUnverifiedQuery : BaseQuery, IRequest<List<PQResponse>>
    {
        
    }
}
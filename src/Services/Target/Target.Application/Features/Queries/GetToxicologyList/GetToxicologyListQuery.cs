using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;
using Target.Application.Features.Queries.CommonVMs;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.GetToxicologyList
{
    public class GetToxicologyListQuery : BaseQuery, IRequest<List<ToxicologyVM>>
    {

    }
}
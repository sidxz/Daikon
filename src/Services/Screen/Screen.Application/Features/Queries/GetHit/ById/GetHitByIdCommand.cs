using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;
using Screen.Domain.Entities;

namespace Screen.Application.Features.Queries.GetHit.ById
{
    public class GetHitByIdCommand : BaseQuery, IRequest<Hit>
    {
        public Guid Id { get; set; }
    }

}
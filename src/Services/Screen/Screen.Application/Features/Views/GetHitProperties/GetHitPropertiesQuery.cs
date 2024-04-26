using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Screen.Application.Features.Views.GetHitProperties
{
    public class GetHitPropertiesQuery : BaseQuery, IRequest<HitPropertiesVM>
    {
        public Guid Id { get; set; }
    }
    
}
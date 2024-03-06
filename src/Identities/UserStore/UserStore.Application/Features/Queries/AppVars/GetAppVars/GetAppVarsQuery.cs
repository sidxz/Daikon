using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace UserStore.Application.Features.Queries.AppVars.GetAppVars
{
    public class GetAppVarsQuery : IRequest<GetAppVarsDTO>
    {
        public Guid UserId { get; set; }
    }
}
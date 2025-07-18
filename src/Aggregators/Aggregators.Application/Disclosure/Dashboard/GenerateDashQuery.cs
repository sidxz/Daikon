using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Aggregators.Application.Disclosure.Dashboard
{
    public class GenerateDashQuery : BaseQuery, IRequest<DisclosureDashView>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
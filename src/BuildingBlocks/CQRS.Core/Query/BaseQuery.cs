using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Query
{
    public abstract class BaseQuery
    {
        public bool WithMeta { get; set; }
        public Guid RequestorUserId { get; set;}
    }
}
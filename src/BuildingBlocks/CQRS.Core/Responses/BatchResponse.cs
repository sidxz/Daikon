using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Responses
{
    public class BatchResponse : BaseResponse
    {
        public List<Guid> Success { get; set; }
        public List<Guid> Failed { get; set; }

    }
}
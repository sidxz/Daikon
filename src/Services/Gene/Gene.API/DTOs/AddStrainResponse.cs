using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gene.API.DTOs
{
    public class AddStrainResponse : BaseResponse
    {
        public Guid Id { get; set; }
    }
}
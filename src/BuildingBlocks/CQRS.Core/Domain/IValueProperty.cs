using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public interface IValueProperty<TDataType>
    {
        TDataType Value { get; set;}
    }
    
}
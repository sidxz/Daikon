using AutoMapper;
using CQRS.Core.Domain;
using System;

namespace CQRS.Core.Resolvers
{
    public class ObjectToDVariableResolver<TSource, TDataType> : IValueResolver<TSource, object, DVariable<TDataType>?>
    {
        private readonly Func<TSource, object> _selector;

        public ObjectToDVariableResolver(Func<TSource, object> selector)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        public DVariable<TDataType>? Resolve(TSource source, object destination, DVariable<TDataType>? destMember, ResolutionContext context)
        {
            var value = _selector(source);

            if (value is DVariable<TDataType> dVariable)
                return dVariable;

            return null;
        }
    }
}

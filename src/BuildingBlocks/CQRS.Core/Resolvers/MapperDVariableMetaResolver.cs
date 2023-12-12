
using AutoMapper;
using CQRS.Core.Domain;

/*
== Summary:
The MapperDVariableMetaResolver class is a generic AutoMapper value resolver.
It is designed to work with properties that implement the IValueProperty<TDataType> interface.
This resolver allows conditional mapping based on the presence and value of a "WithMeta" key in the ResolutionContext's Items dictionary.

== Type Parameters:
TSource: The type of the source object from which the value is to be resolved.
TMember: The type of the member on the source object which implements IValueProperty<TDataType>.
TDataType: The data type of the value held by the TMember type.

== Usage:
This resolver is intended to be used in AutoMapper's mapping configuration to conditionally map properties of the source object to the destination object.
If the "WithMeta" key in the context's Items dictionary is true, the entire TMember object is mapped.
Otherwise, only the Value property of TMember is mapped.

== Example:
CreateMap<SourceType, DestinationType>()
    .ForMember(dest => dest.Property, opt => opt.MapFrom(new MapperDVariableMetaResolver<SourceType, IValueProperty<string>, string>(src => src.Property)));
*/

namespace CQRS.Core.Resolvers
{
    public class MapperDVariableMetaResolver<TSource, TMember, TDataType> : IValueResolver<TSource, object, object>
    where TMember : IValueProperty<TDataType>
    {
        private readonly Func<TSource, TMember> _selector;

        public MapperDVariableMetaResolver(Func<TSource, TMember> selector)
        {
            _selector = selector;
        }

        public object Resolve(TSource source, object destination, object destMember, ResolutionContext context)
        {

            // Check if the source is null
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Source cannot be null.");
            }

            // Check if the selector function is null
            if (_selector == null)
            {
                throw new InvalidOperationException("Selector function cannot be null.");
            }

            var memberValue = _selector(source);

            // Check if the member value is null
            if (memberValue == null)
            {
                // Handle the null case appropriately, return null or a default value
                return default(TDataType); 
            }



            bool withMeta = context.Items.ContainsKey("WithMeta") && (bool)context.Items["WithMeta"];
            return withMeta ? memberValue : memberValue.Value;
        }
    }
}
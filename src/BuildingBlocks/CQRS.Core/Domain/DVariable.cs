using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    /*
     == Overview ==
    The DVariable<TDataType> class is a generic class designed to encapsulate a value of any type (TDataType)
    along with additional metadata inherited from DocMetadata. It provides mechanisms for easy value manipulation
    and comparison, while also supporting JSON serialization behaviors.

    == Design Considerations ==
    1. The class is designed to be generic to accommodate any data type.
    2. It inherits from DocMetadata to include additional metadata.
    3. Implicit operators are provided for ease of use, allowing the class to be used as if it were of type TDataType.
    4. The class handles null values gracefully in ToString, Equals, and GetHashCode methods.
    5. Comparison operators are implemented with a guard clause to ensure TDataType is comparable.

    == Usage ==
    This class can be used to encapsulate any type of data along with metadata. It is particularly useful
    in scenarios where additional information about the data (like source, author, etc.) is required alongside the data itself.
    Example:
     var intVariable = new DVariable<int> { Value = 10 };
     var stringVariable = new DVariable<string>("Hello");

    */

    public class DVariable<TDataType> : DocMetadata, IValueProperty<TDataType>
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TDataType Value { get; set; }

        public DVariable() : base()
        {
            Value = default!;
        }

        /* Copy Constructor */
        public DVariable(DVariable<TDataType> dVariable) : base(dVariable) // Call base copy constructor
        {
            if (dVariable == null)
                throw new ArgumentNullException(nameof(dVariable));

            Value = dVariable.Value;
        }

        public override string ToString()
        {
            // Check if Value is null, which is a possibility for reference types and nullable value types
            if (Value == null)
            {
                return "null";
            }

            // For other types, use the default ToString implementation of the Value
            return Value?.ToString() ?? "null";
        }

        public static implicit operator TDataType(DVariable<TDataType> dVariable)
        {
            return dVariable.Value;
        }

        public static implicit operator DVariable<TDataType>(TDataType value)
        {
            return new DVariable<TDataType> { Value = value };
        }

        public static bool operator ==(DVariable<TDataType> dVariable, TDataType value)
        {
            return dVariable.Value?.Equals(value) ?? false;
        }


        public static bool operator !=(DVariable<TDataType> dVariable, TDataType value)
        {
            return !dVariable.Value?.Equals(value) ?? false;

        }

        public override bool Equals(object? obj)
        {
            if (obj is DVariable<TDataType> dVariable)
            {
                return dVariable.Value?.Equals(Value) ?? false;
            }

            return false;
        }

        // public override int GetHashCode()
        // {
        //     return Value?.GetHashCode() ?? 0;
        // }

        // public static bool operator ==(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        // {
        //     return dVariable1.Value?.Equals(dVariable2.Value) ?? false;
        // }

        // public static bool operator !=(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        // {
        //     return !dVariable1.Value?.Equals(dVariable2.Value) ?? false;
        // }

        // public static bool operator >(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        // {
        //     return dVariable1.Value switch
        //     {
        //         IComparable comparable => comparable.CompareTo(dVariable2.Value) > 0,
        //         _ => throw new InvalidOperationException("Cannot compare values of this type"),
        //     };
        // }

        // public static bool operator <(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        // {
        //     return dVariable1.Value switch
        //     {
        //         IComparable comparable => comparable.CompareTo(dVariable2.Value) < 0,
        //         _ => throw new InvalidOperationException("Cannot compare values of this type"),
        //     };
        // }

        // public static bool operator >=(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        // {
        //     return dVariable1.Value switch
        //     {
        //         IComparable comparable => comparable.CompareTo(dVariable2.Value) >= 0,
        //         _ => throw new InvalidOperationException("Cannot compare values of this type"),
        //     };
        // }

        // public static bool operator <=(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        // {
        //     return dVariable1.Value switch
        //     {
        //         IComparable comparable => comparable.CompareTo(dVariable2.Value) <= 0,
        //         _ => throw new InvalidOperationException("Cannot compare values of this type"),
        //     };
        // }


    }
}
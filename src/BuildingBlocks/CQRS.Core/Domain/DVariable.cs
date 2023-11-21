using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public class DVariable<TDataType> : DocMetadata
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TDataType Value { get; set; }

        public DVariable()
        {
            Value = default!;
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

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        {
            return dVariable1.Value?.Equals(dVariable2.Value) ?? false;
        }

        public static bool operator !=(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        {
            return !dVariable1.Value?.Equals(dVariable2.Value) ?? false;
        }

        public static bool operator >(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        {
            return dVariable1.Value switch
            {
                IComparable comparable => comparable.CompareTo(dVariable2.Value) > 0,
                _ => throw new InvalidOperationException("Cannot compare values of this type"),
            };
        }

        public static bool operator <(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        {
            return dVariable1.Value switch
            {
                IComparable comparable => comparable.CompareTo(dVariable2.Value) < 0,
                _ => throw new InvalidOperationException("Cannot compare values of this type"),
            };
        }

        public static bool operator >=(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        {
            return dVariable1.Value switch
            {
                IComparable comparable => comparable.CompareTo(dVariable2.Value) >= 0,
                _ => throw new InvalidOperationException("Cannot compare values of this type"),
            };
        }

        public static bool operator <=(DVariable<TDataType> dVariable1, DVariable<TDataType> dVariable2)
        {
            return dVariable1.Value switch
            {
                IComparable comparable => comparable.CompareTo(dVariable2.Value) <= 0,
                _ => throw new InvalidOperationException("Cannot compare values of this type"),
            };
        }


    }
}
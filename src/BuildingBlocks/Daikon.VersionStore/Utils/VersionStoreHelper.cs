
using System.Reflection;
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using Microsoft.Extensions.Logging;

namespace Daikon.VersionStore.Utils
{
    /// <summary>
    /// The VersionStoreHelper class provides utility functions to assist with versioning of entities.
    /// It includes methods for identifying DVariable properties, creating new version properties,
    /// and safely setting property values.
    /// </summary>
    /// <typeparam name="VersionEntityModel">The type of the version entity model.</typeparam>
    /// 

    public class VersionStoreHelper<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly ILogger _logger;

        public VersionStoreHelper(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Checks if a given property is of type DVariable<T>.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="genericTypeArgument">Out parameter that returns the generic type argument if the property is of type DVariable<T>.</param>
        /// <returns>True if the property is of type DVariable<T>, otherwise false.</returns>
        public bool IsDVariableProperty(PropertyInfo property, out Type genericTypeArgument)
        {
            try
            {
                genericTypeArgument = null;
                var propertyType = property.PropertyType;

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(DVariable<>))
                {
                    genericTypeArgument = propertyType.GetGenericArguments()[0];
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to check if property is of type DVariable<T>");
                genericTypeArgument = null;
                return false;
            }
        }


        /// <summary>
        /// Finds the equivalent property in the version entity model.
        /// </summary>
        /// <param name="versionEntityModel">The version entity model.</param>
        /// <param name="updatedProperty">The property to find in the version entity model.</param>
        /// <returns>The equivalent property in the version entity model, or null if not found.</returns>

        public PropertyInfo FindVersionProperty(VersionEntityModel versionEntityModel, PropertyInfo updatedProperty)
        {
            var versionProperty = typeof(VersionEntityModel).GetProperty(updatedProperty.Name);
            if (versionProperty == null)
            {
                _logger.LogWarning("Property {UpdatedPropertyName} not found in VersionEntityModel. Versioning will not be maintained for this property.", updatedProperty.Name);
            }

            return versionProperty!;
        }





        /// <summary>
        /// Creates a new instance of DVariableHistory<T>.
        /// </summary>
        /// <param name="genericTypeArgument">The generic type argument for DVariableHistory.</param>
        /// <returns>A new instance of DVariableHistory<T>, or null if creation fails.</returns>
        public object CreateNewVersionProperty(Type genericTypeArgument)
        {
            try
            {
                Type dVariableHistoryType = typeof(DVariableHistory<>).MakeGenericType(genericTypeArgument);
                return Activator.CreateInstance(dVariableHistoryType);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create instance of DVariableHistory");
                return null;
            }
        }



        /// <summary>
        /// Creates a new instance of the VersionEntityModel.
        /// </summary>
        /// <param name="entityId">The entity ID for which the version model is created.</param>
        /// <returns>A new instance of VersionEntityModel.</returns>
        public VersionEntityModel CreateNewVersionModel(Guid entityId)
        {
            try
            {
                var versionEntityModel = Activator.CreateInstance<VersionEntityModel>();
                versionEntityModel.Id = Guid.NewGuid();
                versionEntityModel.EntityId = entityId;
                return versionEntityModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create instance of VersionEntityModel");
                throw;
            }
        }




        /// <summary>
        /// Checks if the value of a property has changed.
        /// </summary>
        /// <param name="updatedPropertyValue">The updated property value.</param>
        /// <param name="versionPropertyValue">The existing property value in the version model.</param>
        /// <returns>True if the value has changed, otherwise false.</returns>
        public bool HasValueChanged(object updatedPropertyValue, object versionPropertyValue)
        {
            // Safely get the CurrentValue property from versionPropertyValue
            var currentValueProperty = versionPropertyValue.GetType().GetProperty("CurrentValue");
            var currentValue = currentValueProperty != null ? currentValueProperty.GetValue(versionPropertyValue) : null;

            // Safely get the Value property from updatedPropertyValue
            var updatedValueProperty = updatedPropertyValue.GetType().GetProperty("Value");
            var updatedValue = updatedValueProperty != null ? updatedValueProperty.GetValue(updatedPropertyValue) : null;

            // Compare the values, handling potential nulls
            return !Equals(currentValue, updatedValue);
        }



        /// <summary>
        /// Safely sets a property value in the version model.
        /// </summary>
        /// <param name="target">The target object on which to set the property.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set on the property.</param>
        public void SetPropertySafe(object target, string propertyName, object value)
        {

            var property = target.GetType().GetProperty(propertyName);

            if (property != null)
            {
                if (property.CanWrite)
                {
                    try
                    {
                        // Ensure the value is compatible with the property type
                        if (value == null || property.PropertyType.IsAssignableFrom(value.GetType()))
                        {
                            property.SetValue(target, value);
                        }
                        else
                        {
                            _logger.LogWarning("Type mismatch when setting property {PropertyName} on {TargetType}. Expected type {ExpectedType}.", propertyName, target.GetType().Name, property.PropertyType.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error setting property {PropertyName} on {TargetType}: {ErrorMessage}", propertyName, target.GetType().Name, e.Message);
                    }
                }
                else
                {
                    _logger.LogWarning("Property {PropertyName} on {TargetType} is read-only.", propertyName, target.GetType().Name);
                }
            }
            else
            {
                _logger.LogError("Property {PropertyName} not found in {TargetType}.", propertyName, target.GetType().Name);
            }
        }

    }
}
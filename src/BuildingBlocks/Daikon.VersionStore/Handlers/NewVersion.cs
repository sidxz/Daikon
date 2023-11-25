using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using Microsoft.Extensions.Logging;

namespace Daikon.VersionStore.Handlers
{
    public class NewVersion<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly ILogger<VersionMaintainer<VersionEntityModel>> _logger;
        public NewVersion(ILogger<VersionMaintainer<VersionEntityModel>> logger)
        {
            _logger = logger;
        }

        public VersionEntityModel Create(BaseEntity updatedEntity)
        {
            // New model
            // Create a new version model
            var newVersionModel = Activator.CreateInstance<VersionEntityModel>();
            newVersionModel.Id = Guid.NewGuid();
            if (updatedEntity.Id == Guid.Empty)
            {
                throw new Exception("Entity Id cannot be empty");
            }

            newVersionModel.EntityId = updatedEntity.Id;

            var updatedEntityType = updatedEntity.GetType();

            // Loop through each property and add it to the version model
            foreach (var updatedProperty in updatedEntityType.GetProperties())
            {

                var updatedProperty_Value = updatedProperty.GetValue(updatedEntity);
                var updatedProperty_Type = updatedProperty.PropertyType;

                // Check if the property is of type DVariable<T>; We only record changes to DVariable<T> properties

                if (updatedProperty_Type != null && updatedProperty_Type.IsGenericType && updatedProperty_Type.GetGenericTypeDefinition() == typeof(DVariable<>))
                {


                    // Find the equivalent property value in the version model
                    var _DVariableHistory = typeof(VersionEntityModel).GetProperty(updatedProperty.Name);
                    if (_DVariableHistory == null)
                    {
                        _logger.LogWarning("Property not found in VersionEntityModel. " + updatedProperty.Name);
                        _logger.LogWarning("Versioning will not be maintained for this DVariable property");
                        continue;
                    }

                    var _DVariableHistory_Value = _DVariableHistory.GetValue(newVersionModel);
                    //var _DVariableHistory_Type = _DVariableHistory.PropertyType;

                    if (_DVariableHistory_Value != null)
                    {
                        throw new Exception("DVariableHistory property already exists in VersionEntityModel. " + updatedProperty.Name);
                    }


                    Type genericTypeArgument = _DVariableHistory.PropertyType.GetGenericArguments()[0];

                    // Create a new instance of DVariableHistory<T> to store the updated value

                    _logger.LogInformation("Create a new instance of DVariableHistory<T> to store the updated value");

                    try
                    {
                        Type dVariableHistoryType = typeof(DVariableHistory<>).MakeGenericType(genericTypeArgument);
                        _DVariableHistory_Value = Activator.CreateInstance(dVariableHistoryType);
                        _logger.LogInformation("_DVariableHistory_Value_New Created");
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Failed to create instance of DVariableHistory");
                        _logger.LogInformation(e.ToString());
                    }


                    // Set the CurrentVersion of _DVariableHistory_Value_New to 1
                    try
                    {
                        _DVariableHistory_Value?.GetType()?.GetProperty("CurrentVersion")?.SetValue(_DVariableHistory_Value, 1);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting CurrentVersion");
                        _logger.LogInformation(e.ToString());
                    }


                    // Set the CurrentValue of _DVariableHistory to the updatedPropertyValue
                    try
                    {
                        Type dVariableType = typeof(DVariable<>).MakeGenericType(genericTypeArgument);
                        var updatedPropertyValue = Activator.CreateInstance(dVariableType, updatedProperty_Value);
                        _DVariableHistory_Value?.GetType()?.GetProperty("Current")?.SetValue(_DVariableHistory_Value, updatedPropertyValue);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting CurrentValue");
                        _logger.LogInformation(e.ToString());
                    }

                    // Set the CurrentValue
                    try
                    {
                        _DVariableHistory_Value?.GetType()?.GetProperty("CurrentValue")?
                            .SetValue(_DVariableHistory_Value, updatedProperty_Value.GetType()
                            .GetProperty("Value"));
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting CurrentValue Object");
                        _logger.LogInformation(e.ToString());
                    }


                    // Add current version to the Versions list
                    var _versionsProperty = _DVariableHistory_Value?.GetType().GetProperty("Versions");
                    var _versionsList = _versionsProperty?.GetValue(_DVariableHistory_Value);
                    
                    // Create Version Entry
                    try
                    {

                        Type _VersionType = typeof(VersionEntry<>).MakeGenericType(genericTypeArgument);
                        var _VersionValue = Activator.CreateInstance(_VersionType);

                        Type dVariableType = typeof(DVariable<>).MakeGenericType(genericTypeArgument);
                        var updatedPropertyValue = Activator.CreateInstance(dVariableType, updatedProperty_Value);
                    
                        _VersionValue.GetType().GetProperty("VersionNumber").SetValue(_VersionValue, 1);
                        _VersionValue.GetType().GetProperty("VersionDetails").SetValue(_VersionValue, updatedPropertyValue);


                        _versionsList.GetType().GetMethod("Add").Invoke(_versionsList, [_VersionValue]);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while adding version entry to versions list");
                        _logger.LogInformation(e.ToString());
                    }

                    // Set it on the newVersionModel
                    _DVariableHistory.SetValue(newVersionModel, _DVariableHistory_Value);

                }


            }
            return newVersionModel;
        }
    }
}
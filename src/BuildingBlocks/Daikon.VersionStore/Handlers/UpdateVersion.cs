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
    public class UpdateVersion<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly ILogger<VersionMaintainer<VersionEntityModel>> _logger;
        public UpdateVersion(ILogger<VersionMaintainer<VersionEntityModel>> logger)
        {
            _logger = logger;
        }

        public VersionEntityModel Update(BaseEntity updatedEntity, VersionEntityModel versionEntityModel)
        {
            // New model
            // Create a new version model

            if (versionEntityModel == null)
            {
                throw new Exception("VersionEntityModel cannot be null");
            }


            if (updatedEntity.Id == Guid.Empty)
            {
                throw new Exception("Entity Id cannot be empty");
            }

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

                    var _DVariableHistory_Value = _DVariableHistory.GetValue(versionEntityModel);
                    //var _DVariableHistory_Type = _DVariableHistory.PropertyType;

                    Type genericTypeArgument = _DVariableHistory.PropertyType.GetGenericArguments()[0];

                    if (_DVariableHistory_Value == null)
                    {
                        _logger.LogWarning("DVariableHistory property not found in VersionEntityModel. " + updatedProperty.Name);
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
                    }

                    // See if the property has changed
                    var _DVariableHistory_Value_CurrentValue = _DVariableHistory_Value.GetType().GetProperty("CurrentValue").GetValue(_DVariableHistory_Value);
                    var _DVariableUpdatedEntity_Value_CurrentValue = updatedProperty_Value.GetType().GetProperty("Value").GetValue(updatedProperty_Value);

                    if (_DVariableHistory_Value_CurrentValue.Equals(_DVariableUpdatedEntity_Value_CurrentValue))
                    {
                        _logger.LogInformation("No change in value for " + updatedProperty.Name);
                        continue;
                    }

                    // Value has changed


                    // Update the current version
                    int updatedVersion = 0;
                    try
                    {
                        var existingVersion = (int)_DVariableHistory_Value?.GetType()?.GetProperty("CurrentVersion")?.GetValue(_DVariableHistory_Value);
                        updatedVersion = existingVersion + 1;
                        _DVariableHistory_Value?.GetType()?.GetProperty("CurrentVersion")?.SetValue(_DVariableHistory_Value, updatedVersion);
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
                        var updatedValue = updatedProperty_Value.GetType().GetProperty("Value").GetValue(updatedProperty_Value);

                        _DVariableHistory_Value?.GetType()?.GetProperty("CurrentValue")?
                            .SetValue(_DVariableHistory_Value, updatedValue);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting CurrentValue Object");
                        _logger.LogInformation(e.ToString());
                    }

                    // Mark IsInitialVersion false
                    try
                    {
                        _DVariableHistory_Value?.GetType()?.GetProperty("IsInitialVersion")?.SetValue(_DVariableHistory_Value, false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting IsInitialVersion");
                        _logger.LogInformation(e.ToString());
                    }

                    // Update CurrentModificationDate
                    try
                    {
                        _DVariableHistory_Value?.GetType()?.GetProperty("CurrentModificationDate")?.SetValue(_DVariableHistory_Value, DateTime.UtcNow);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting CurrentModificationDate");
                        _logger.LogInformation(e.ToString());
                    }

                    // Update CurrentAuthor
                    try
                    {
                        var currentAuthor = updatedProperty_Value.GetType().GetProperty("Author").GetValue(updatedProperty_Value);
                        _DVariableHistory_Value?.GetType()?.GetProperty("CurrentAuthor")?.SetValue(_DVariableHistory_Value, currentAuthor);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while setting CurrentAuthor");
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

                        _VersionValue.GetType().GetProperty("VersionNumber").SetValue(_VersionValue, updatedVersion);
                        _VersionValue.GetType().GetProperty("VersionDetails").SetValue(_VersionValue, updatedPropertyValue);


                        _versionsList.GetType().GetMethod("Add").Invoke(_versionsList, [_VersionValue]);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("Exception while adding version entry to versions list");
                        _logger.LogInformation(e.ToString());
                    }

                    // Set it on the newVersionModel
                    _DVariableHistory.SetValue(versionEntityModel, _DVariableHistory_Value);

                }


            }
            return versionEntityModel;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Logging;

namespace Daikon.VersionStore.Handlers
{
    public class VersionMaintainer<VersionEntityModel> : IVersionMaintainer<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly IVersionStoreRepository<VersionEntityModel> _versionStoreRepository;
        private readonly ILogger<VersionMaintainer<VersionEntityModel>> _logger;

        public VersionMaintainer(IVersionStoreRepository<VersionEntityModel> versionStoreRepository, ILogger<VersionMaintainer<VersionEntityModel>> logger)
        {
            _versionStoreRepository = versionStoreRepository;
            _logger = logger;
        }

        public async Task CommitVersion(BaseEntity updatedEntity)
        {
            var existingModel = await _versionStoreRepository.GetByAsyncEntityId(updatedEntity.Id).ConfigureAwait(false);
            if (existingModel == null)
            {
                // New model
                // Create a new version model
                var newVersionModel = Activator.CreateInstance<VersionEntityModel>();
                newVersionModel.Id = Guid.NewGuid();
                newVersionModel.EntityId = updatedEntity.Id;

                var updatedEntityType = updatedEntity.GetType();
                _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++++ entityType");
                _logger.LogInformation(updatedEntityType.ToString());
                _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++++ entityType.GetProperties()");
                // loop and print all properties
                // foreach (var property in entityType.GetProperties())
                // {
                //     _logger.LogInformation(property.Name);
                //     // get the value of the property and print it
                //     var propertyValue = property.GetValue(updatedEntity);
                //     _logger.LogInformation(JsonSerializer.Serialize(propertyValue));
                // }


                // Loop through each property and add it to the version model
                foreach (var updatedProperty in updatedEntityType.GetProperties())
                {
                    _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++++ For each");
                    var updatedProperty_Value = updatedProperty.GetValue(updatedEntity);
                    var updatedProperty_Type = updatedProperty.PropertyType;
                    _logger.LogInformation(updatedProperty.Name + " | " + updatedProperty_Type.ToString());
                    _logger.LogInformation(JsonSerializer.Serialize(updatedProperty_Value)); // JJ9,  {"Value":"Func 1"}
                    _logger.LogInformation("-------------------------------------------");


                    // Check if the property is of type DVariable<T>
                    if (updatedProperty_Type != null && updatedProperty_Type.IsGenericType && updatedProperty_Type.GetGenericTypeDefinition() == typeof(DVariable<>))
                    {
                        _logger.LogInformation("Passed if -> updatedPropertyType Identified as generic DVariable");

                        // Find the equivalent property value in the version model
                        var _DVariableHistory = typeof(VersionEntityModel).GetProperty(updatedProperty.Name);
                        _logger.LogInformation("_DVariableHistory.Name");
                        _logger.LogInformation(_DVariableHistory.Name);
                        var _DVariableHistory_Value = _DVariableHistory.GetValue(newVersionModel);
                        _logger.LogInformation("_DVariableHistory_Value");
                        _logger.LogInformation(JsonSerializer.Serialize(_DVariableHistory_Value));
                        var _DVariableHistory_Type = _DVariableHistory.PropertyType;
                        _logger.LogInformation("_DVariableHistory_Type");
                        _logger.LogInformation(_DVariableHistory_Type.ToString());

                        _logger.LogInformation("-----");

                        // Create a new instance of DVariableHistory<T> to store the updated value

                        if (_DVariableHistory_Value == null)
                        {
                            _logger.LogInformation("_DVariableHistory_Value is null");
                            _logger.LogInformation("Create a new instance of DVariableHistory<T> to store the updated value");

                            Type genericTypeArgument = _DVariableHistory.PropertyType.GetGenericArguments()[0];
                            Type dVariableHistoryType = typeof(DVariableHistory<>).MakeGenericType(genericTypeArgument);
                            _logger.LogInformation("dVariableHistoryType");
                            _logger.LogInformation(dVariableHistoryType.ToString());

                            _DVariableHistory_Value = Activator.CreateInstance(dVariableHistoryType);
                            _logger.LogInformation("_DVariableHistory_Value_New Created");
                        }
                        else
                        {
                            _logger.LogInformation("_DVariableHistory_Value is not null. Using Existing instance");
                        }




                        // Set the CurrentVersion of _DVariableHistory_Value_New to 1
                        _logger.LogInformation("Set the CurrentVersion of _DVariableHistory_Value_New to 1");
                        _DVariableHistory_Value.GetType().GetProperty("CurrentVersion").SetValue(_DVariableHistory_Value, 1);

                        // Set the CurrentValue of _DVariableHistory to the updatedPropertyValue
                        _logger.LogInformation("Set the CurrentValue of _DVariableHistory to the updatedPropertyValue");
                        try
                        {
                            Type genericTypeArgument = _DVariableHistory.PropertyType.GetGenericArguments()[0];
                            Type dVariableType = typeof(DVariable<>).MakeGenericType(genericTypeArgument);
                            var updatedPropertyValue = Activator.CreateInstance(dVariableType, updatedProperty_Value);
                            _DVariableHistory_Value.GetType().GetProperty("CurrentValue").SetValue(_DVariableHistory_Value, updatedPropertyValue);
                        }
                        catch (Exception e)
                        {
                            _logger.LogInformation("Exception");
                            _logger.LogInformation(e.ToString());
                        }


                        // Add current version to the Versions list
                        var _versionsProperty = _DVariableHistory_Value.GetType().GetProperty("Versions");
                        var _versionsList = _versionsProperty.GetValue(_DVariableHistory_Value);
                        if (_versionsList == null)
                        {
                            _logger.LogInformation("_versionsList is null");
                            Type listType = typeof(List<>).MakeGenericType(_versionsProperty.PropertyType.GetGenericArguments()[0]);
                            _versionsList = Activator.CreateInstance(listType);
                            _versionsProperty.SetValue(_DVariableHistory_Value, _versionsList);
                            _logger.LogInformation("_versionsList_New Created");
                        }
                        else
                        {
                            _logger.LogInformation("_versionsList is not null. Using Existing instance");
                        }


                        // Create Version Entry
                        try
                        {
                            Type genericTypeArgument = _DVariableHistory.PropertyType.GetGenericArguments()[0];
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
                            _logger.LogInformation("Exception");
                            _logger.LogInformation(e.ToString());
                        }




                        




                        // Set it on the newVersionModel
                        _logger.LogInformation("Set it on the newVersionModel");
                        _DVariableHistory.SetValue(newVersionModel, _DVariableHistory_Value);

                        // lets print the present model for debug
                        _logger.LogInformation("lets print the present model for debug");
                        _logger.LogInformation(JsonSerializer.Serialize(newVersionModel));















                    }


                }
                _logger.LogInformation("End of for each");
                _logger.LogInformation("Save to db");
                // Add the version model to the database
                await _versionStoreRepository.SaveAsync(newVersionModel).ConfigureAwait(false);
                _logger.LogInformation("End of Save to db");

            }

            else
            {
                // Existing model
                // Loop through each property and check if it has changed
                foreach (var property in typeof(BaseEntity).GetProperties())
                {
                    var propertyEntry = property.GetValue(updatedEntity);

                    // Check if the property is of type DVariable<T>
                    if (propertyEntry is DVariable<object> dVariable_updatedEntity)
                    {
                        //  Updated Value of DVariableHistory
                        var updatedValue = dVariable_updatedEntity.Value;

                        // Get the existing value for comparison
                        var versionPropertyEntry = typeof(VersionEntityModel).GetProperty(property.Name);
                        var dVariableHistory_versionModel = versionPropertyEntry?.GetValue(existingModel) as DVariableHistory<object>;
                        var existingValue = dVariableHistory_versionModel?.CurrentValue;

                        if (!Equals(existingValue, updatedValue))
                        {
                            // The property has changed
                            // Handle the change accordingly
                            // ...

                            // 1. Update Version
                            var newVersion = dVariableHistory_versionModel.CurrentVersion + 1;
                            dVariableHistory_versionModel.CurrentVersion = newVersion;

                            // 2. Update Value
                            dVariableHistory_versionModel.CurrentValue = updatedValue;

                            // 3. Update UpdatedAt
                            dVariableHistory_versionModel.CurrentModificationDate = DateTime.UtcNow;

                            // 4 Insert new version
                            dVariableHistory_versionModel.Versions.Add(new VersionEntry<object>
                            {
                                VersionNumber = newVersion,
                                VersionDetails = dVariable_updatedEntity
                            });



                        }
                    }

                }
                // Save the updated model
                await _versionStoreRepository.UpdateAsync(existingModel).ConfigureAwait(false);

            }


        }

    }
}
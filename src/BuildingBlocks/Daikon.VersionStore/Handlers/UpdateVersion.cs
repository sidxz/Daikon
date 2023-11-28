
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using Daikon.VersionStore.Utils;
using Microsoft.Extensions.Logging;

/// <summary>
/// The UpdateVersion class provides functionality to update version information for entities derived from BaseVersionEntity.
/// It is designed to work with entities that use DVariable<T> properties to track changes over time. The class
/// encapsulates the logic for incrementing version numbers, updating modification dates, authors, and other relevant
/// versioning details. It uses reflection to dynamically handle properties of the provided entity and updates
/// corresponding properties in the version entity model.
///
/// Usage:
/// - Intended to be used in scenarios where version tracking of entity properties is required.
/// - Works with entities that have properties of type DVariable<T>, updating corresponding DVariableHistory<T>
///   properties in the version entity model.
///
/// Implementation Details:
/// - The class uses generics to handle any entity type derived from BaseVersionEntity.
/// - Reflection is used to inspect and update properties dynamically, allowing for flexibility with different entity structures.
/// - The Update method is the primary entry point, taking an updated entity and the corresponding version entity model
///   as parameters.
/// - The class includes several private helper methods to modularize and simplify the logic, such as validating input,
///   checking property types, and updating version property values.
///
/// Note:
/// - The class assumes the existence of specific properties and methods in the DVariableHistory<T> and VersionEntry<T>
///   classes. Adjustments may be needed based on the actual structure of these classes.
/// - Proper exception handling and logging are implemented to ensure robustness and ease of debugging.
/// </summary>

namespace Daikon.VersionStore.Handlers
{
    public class UpdateVersion<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly ILogger<VersionMaintainer<VersionEntityModel>> _logger;
        private readonly VersionStoreHelper<VersionEntityModel> _versionStoreHelper;

        public UpdateVersion(ILogger<VersionMaintainer<VersionEntityModel>> logger)
        {
            _logger = logger;
            _versionStoreHelper = new VersionStoreHelper<VersionEntityModel>(logger);
        }

        /*
            * Validate the input
            * Throw exception if input is invalid
        */
        private void ValidateInput(BaseEntity updatedEntity, VersionEntityModel versionEntityModel)
        {
            if (versionEntityModel == null)
                throw new ArgumentNullException(nameof(versionEntityModel), "VersionEntityModel cannot be null");

            if (updatedEntity.Id == Guid.Empty)
                throw new ArgumentException("Entity Id cannot be empty", nameof(updatedEntity));
        }


        public VersionEntityModel Update(BaseEntity updatedEntity, VersionEntityModel versionEntityModel)
        {
            ValidateInput(updatedEntity, versionEntityModel);

            // Loop through each property and add it to the version model
            foreach (var updatedProperty in updatedEntity.GetType().GetProperties())
            {

                // Check if the property is of type DVariable<T>; We only record changes to DVariable<T> properties

                if (_versionStoreHelper.IsDVariableProperty(updatedProperty, out Type genericTypeArgument))
                {
                    _logger.LogDebug("Property {PropertyName} is of type DVariable<T>", updatedProperty.Name);


                    var versionProperty = _versionStoreHelper.FindVersionProperty(versionEntityModel, updatedProperty); //_DVariableHistory
                    if (versionProperty == null) continue;

                    var versionPropertyValue = versionProperty.GetValue(versionEntityModel); // _DVariableHistory_Value
                    var updatedPropertyValue = updatedProperty.GetValue(updatedEntity);

                    _logger.LogInformation("versionPropertyValue: " + versionPropertyValue);
                    _logger.LogInformation("updatedPropertyValue: " + updatedPropertyValue);


                    if (versionPropertyValue == null)
                    {
                        _logger.LogInformation("DVariableHistory property for {DVarName} not found in VersionEntityModel. Will create new.", updatedProperty.Name);
                        versionPropertyValue = _versionStoreHelper.CreateNewVersionProperty(genericTypeArgument);
                        versionProperty.SetValue(versionEntityModel, versionPropertyValue);
                    }


                    // Proceed only if the value has changed
                    if (!_versionStoreHelper.HasValueChanged(updatedPropertyValue, versionPropertyValue))
                    {
                        _logger.LogDebug("No change in value for {DVarName}", updatedProperty.Name);
                        continue;
                    }

                    // Increment the CurrentVersion
                    int updatedVersion = 0;
                    try
                    {
                        var currentVersionProperty = versionPropertyValue!.GetType().GetProperty("CurrentVersion");
                        var existingVersionObj = currentVersionProperty?.GetValue(versionPropertyValue);
                        int existingVersion = existingVersionObj is int existingInt ? existingInt : 0; // Default to 0 if null or not an int
                        updatedVersion = existingVersion + 1;

                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "CurrentVersion", updatedVersion);

                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentVersion {ExceptionMessage}", e.ToString());
                    }


                    // Set the "Current" to the updated value
                    Type dVariableType = typeof(DVariable<>).MakeGenericType(genericTypeArgument);
                    var current = Activator.CreateInstance(dVariableType, updatedPropertyValue);
                    try
                    {
                        if (current == null)
                        {
                            _logger.LogError("Failed to create instance of DVariable");
                            throw new Exception("Failed to create instance of DVariable");
                        }
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "Current", current);
                    }

                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentValue: {ExceptionMessage}", e.ToString());
                    }

                    // Set the CurrentValue to the updated value *unwrapped*
                    try
                    {
                        var currentValue = updatedPropertyValue!.GetType()?.GetProperty("Value")?.GetValue(updatedPropertyValue);
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "CurrentValue", currentValue!);

                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentValue Object: {ExceptionMessage}", e.ToString());
                    }

                    // Set IsInitialVersion to false
                    try
                    {
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "IsInitialVersion", false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting IsInitialVersion to false: {ExceptionMessage}", e.ToString());
                    }

                    // Set CurrentModificationDate to DateTime.UtcNow
                    try
                    {
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "CurrentModificationDate", DateTime.UtcNow);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentModificationDate: {ExceptionMessage}", e.ToString());
                    }

                    // Set CurrentAuthor to Author
                    try
                    {
                        var currentAuthor = updatedPropertyValue?.GetType().GetProperty("Author")?.GetValue(updatedPropertyValue) ?? null;
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "CurrentAuthor", currentAuthor!);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentAuthor: {ExceptionMessage}", e.ToString());
                    }

                    // Add a new VersionEntry to the Versions list
                    // Get the Versions list
                    var versionsListProp = versionPropertyValue?.GetType().GetProperty("Versions");
                    var versionsList = versionsListProp?.GetValue(versionPropertyValue);
                    var addMethod = versionsList?.GetType().GetMethod("Add");

                    if (versionsList == null || addMethod == null)
                    {
                        _logger.LogError("Versions list not found");
                        throw new Exception("Versions list not found");
                    }


                    // Create a new VersionEntry<T>
                    try
                    {
                        Type versionEntryType = typeof(VersionEntry<>).MakeGenericType(genericTypeArgument);
                        var versionEntry = Activator.CreateInstance(versionEntryType);


                        _versionStoreHelper.SetPropertySafe(versionEntry, "VersionNumber", updatedVersion);
                        _versionStoreHelper.SetPropertySafe(versionEntry, "VersionDetails", current);

                        // Add it to the Versions list 
                        addMethod.Invoke(versionsList, new[] { versionEntry });

                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while adding version entry to versions list: {ExceptionMessage}", e.ToString());
                    }

                    // Set the property value in the version model
                    // SetPropertySafe(versionEntityModel, updatedProperty.Name, versionPropertyValue);
                    versionProperty.SetValue(versionEntityModel, versionPropertyValue);
                }
            }
            return versionEntityModel;
        }
    }
}
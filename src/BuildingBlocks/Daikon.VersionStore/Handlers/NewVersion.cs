
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using Daikon.VersionStore.Utils;
using Microsoft.Extensions.Logging;

namespace Daikon.VersionStore.Handlers
{
    /// <summary>
    /// == Overview
    /// The NewVersion class is designed to create new version models for entities derived from BaseVersionEntity. 
    /// It primarily handles entities with DVariable<T> properties, creating a new version history for each of these properties. 
    /// The class uses reflection to dynamically interact with entity properties, ensuring flexibility and adaptability to different entity structures.
    /// </summary>

    public class NewVersion<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly ILogger<VersionHub<VersionEntityModel>> _logger;
        private readonly VersionStoreHelper<VersionEntityModel> _versionStoreHelper;
        public NewVersion(ILogger<VersionHub<VersionEntityModel>> logger)
        {
            _logger = logger;
            _versionStoreHelper = new VersionStoreHelper<VersionEntityModel>(logger);
        }

        /*
            * Validate the input
            * Throw exception if input is invalid
        */
        private void ValidateInput(BaseEntity updatedEntity)
        {
            if (updatedEntity.Id == Guid.Empty)
                throw new ArgumentException("Entity Id cannot be empty", nameof(updatedEntity));
        }

        public VersionEntityModel Create(BaseEntity updatedEntity)
        {

            ValidateInput(updatedEntity);


            // Create a new version model
            var versionEntityModel = _versionStoreHelper.CreateNewVersionModel(updatedEntity.Id);


            // Loop through each property and add it to the version model
            foreach (var updatedProperty in updatedEntity.GetType().GetProperties())
            {

                // Check if the property is of type DVariable<T>; We only record changes to DVariable<T> properties
                if (_versionStoreHelper.IsDVariableProperty(updatedProperty, out Type genericTypeArgument))
                {
                    _logger.LogDebug("Property {PropertyName} is of type DVariable<T>", updatedProperty.Name);


                    // Find the equivalent property value in the version model
                    var versionProperty = _versionStoreHelper.FindVersionProperty(versionEntityModel, updatedProperty);
                    if (versionProperty == null) continue;

                    var versionPropertyValue = versionProperty.GetValue(versionEntityModel);
                    var updatedPropertyValue = updatedProperty.GetValue(updatedEntity);

                    if (versionPropertyValue != null)
                    {
                        throw new Exception("DVariableHistory property already exists in VersionEntityModel. " + updatedProperty.Name);
                    }

                    if(updatedPropertyValue == null)
                    {
                        _logger.LogDebug("Property {PropertyName} is null", updatedProperty.Name);
                        continue;
                    }


                    // Create a new instance of DVariableHistory<T> to store the updated value
                    _logger.LogDebug("Create a new instance of DVariableHistory<T> to store the updated value");

                    versionPropertyValue = _versionStoreHelper.CreateNewVersionProperty(genericTypeArgument);
                    versionProperty.SetValue(versionEntityModel, versionPropertyValue);


                    // Set the CurrentVersion of _DVariableHistory_Value_New to 1
                    _logger.LogDebug("Set the CurrentVersion of _DVariableHistory_Value_New to 1");
                    try
                    {
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "CurrentVersion", 1);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentVersion {ExceptionMessage}", e.ToString());
                    }


                    // Set the "Current" to the updated value
                    _logger.LogDebug("Set the Current to the updated value");
                    Type dVariableType = typeof(DVariable<>).MakeGenericType(genericTypeArgument);
                    _logger.LogDebug("DVariableType: {DVariableType}", dVariableType);
                    object current;
                    try
                    {
                        current = Activator.CreateInstance(dVariableType, updatedPropertyValue);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while creating instance of DVariable: {ExceptionMessage}", e.ToString());
                        throw;
                    }
                    
                    _logger.LogDebug("Current: {Current}", current);
                    try
                    {
                        if (current == null)
                        {
                            _logger.LogError("Failed to create instance of DVariable");
                            throw new Exception("Failed to create instance of DVariable");
                        }
                        _logger.LogDebug("SetPropertySafe the CurrentValue to the updated value");
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "Current", current);
                    }

                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentValue: {ExceptionMessage}", e.ToString());
                    }

                    // Set the CurrentValue to the updated value *unwrapped*
                    _logger.LogDebug("Set the CurrentValue to the updated value *unwrapped*");
                    try
                    {
                        var currentValue = updatedPropertyValue!.GetType()?.GetProperty("Value")?.GetValue(updatedPropertyValue);
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "CurrentValue", currentValue!);

                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Exception while setting CurrentValue Object: {ExceptionMessage}", e.ToString());
                    }

                    // Set IsInitialVersion to true
                    try
                    {
                        _versionStoreHelper.SetPropertySafe(versionPropertyValue, "IsInitialVersion", true);
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
                    _logger.LogDebug("Add a new VersionEntry to the Versions list");
                    // Get the Versions list
                    var versionsListProp = versionPropertyValue?.GetType().GetProperty("Versions");
                    var versionsList = versionsListProp?.GetValue(versionPropertyValue);
                    var addMethod = versionsList?.GetType().GetMethod("Add");

                    _logger.LogDebug("Versions list: {VersionsList}", versionsList);
                    if (versionsList == null || addMethod == null)
                    {
                        _logger.LogError("Versions list not found");
                        throw new Exception("Versions list not found");
                    }


                    // Create a new VersionEntry<T>
                    _logger.LogDebug("Create a new VersionEntry<T>");
                    try
                    {
                        Type versionEntryType = typeof(VersionEntry<>).MakeGenericType(genericTypeArgument);
                        var versionEntry = Activator.CreateInstance(versionEntryType);


                        _versionStoreHelper.SetPropertySafe(versionEntry, "VersionNumber", 1);
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
                    _logger.LogDebug("Set the property value in the version model");
                    versionProperty.SetValue(versionEntityModel, versionPropertyValue);

                }

            }
            return versionEntityModel;
        }
    }
}
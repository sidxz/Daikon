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

                NewVersion<VersionEntityModel> newVersion = new(_logger);
                var newVersionModel = newVersion.Create(updatedEntity);
               
                await _versionStoreRepository.SaveAsync(newVersionModel).ConfigureAwait(false);
                
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
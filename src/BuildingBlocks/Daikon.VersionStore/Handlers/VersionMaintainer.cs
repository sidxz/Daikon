
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Logging;

namespace Daikon.VersionStore.Handlers
{
    /// <summary>
    /// The VersionMaintainer class is responsible for maintaining the version history of entities.
    /// It handles the creation of new version models and the updating of existing ones.
    /// </summary>
    /// <typeparam name="VersionEntityModel">The type of the version entity model.</typeparam>
    /// 
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
            // Retrieve the existing version model for the entity
            var existingModel = await _versionStoreRepository.GetByAsyncEntityId(updatedEntity.Id).ConfigureAwait(false);

            if (existingModel == null)
            {
                // If no existing model, create a new version model
                NewVersion<VersionEntityModel> newVersion = new(_logger);
                var newVersionModel = newVersion.Create(updatedEntity);

                // Save the new model
                try
                {
                    await _versionStoreRepository.SaveAsync(newVersionModel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving version model");
                }


            }
            else
            {
                // If an existing model is found, update it
                UpdateVersion<VersionEntityModel> updateVersion = new(_logger);
                var updatedVersionModel = updateVersion.Update(updatedEntity, existingModel);

                // Save the updated model
                try
                {
                    await _versionStoreRepository.UpdateAsync(updatedVersionModel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating version model");
                }

            }
        }

    }
}

using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Microsoft.Extensions.Logging;

namespace Daikon.VersionStore.Handlers
{
    /// <summary>
    /// The VersionHub class is responsible for maintaining the version history of entities.
    /// It handles the creation of new version models and the updating of existing ones.
    /// </summary>
    /// <typeparam name="VersionEntityModel">The type of the version entity model.</typeparam>
    /// 
    public class VersionHub<VersionEntityModel> : IVersionHub<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        private readonly IVersionStoreRepository<VersionEntityModel> _versionStoreRepository;
        private readonly ILogger<VersionHub<VersionEntityModel>> _logger;

        public VersionHub(IVersionStoreRepository<VersionEntityModel> versionStoreRepository, ILogger<VersionHub<VersionEntityModel>> logger)
        {
            _versionStoreRepository = versionStoreRepository;
            _logger = logger;
        }

        public async Task CommitVersion(BaseEntity updatedEntity)
        {
            // Retrieve the existing version model for the entity
            _logger.LogDebug("Retrieving existing version model for entity with id {EntityId}", updatedEntity.Id);
            var existingModel = await _versionStoreRepository.GetByAsyncEntityId(updatedEntity.Id).ConfigureAwait(false);

            if (existingModel == null)
            {
                // If no existing model, create a new version model
                _logger.LogDebug("No existing version model found for entity with id {EntityId}", updatedEntity.Id);
                NewVersion<VersionEntityModel> newVersion = new(_logger);
                var newVersionModel = newVersion.Create(updatedEntity);

                // Save the new model
                try
                {
                    _logger.LogDebug("Saving new version model for entity with id {EntityId}", updatedEntity.Id);
                    await _versionStoreRepository.SaveAsync(newVersionModel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving version model");
                }


            }
            else
            {
                // check if the entity has been deleted
                if (existingModel.IsEntityDeleted)
                {
                    _logger.LogWarning("Entity with id {EntityId} has been deleted", updatedEntity.Id);
                    throw new EntityDeletedException("The parent entity is deleted. Cannot alter version history");
                }
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

        public async Task<VersionEntityModel> GetVersions(Guid entityId)
        {
           return await _versionStoreRepository.GetByAsyncEntityId(entityId).ConfigureAwait(false);
        }

        public async Task ArchiveEntity(Guid entityId)
        {
            var existingModel = await _versionStoreRepository.GetByAsyncEntityId(entityId).ConfigureAwait(false);

            if (existingModel == null)
            {
                _logger.LogWarning("Entity with id {EntityId} not found", entityId);
            }

            existingModel.IsEntityDeleted = true;

            try
            {
                await _versionStoreRepository.UpdateAsync(existingModel).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating version model");
            }
        }
    }
}
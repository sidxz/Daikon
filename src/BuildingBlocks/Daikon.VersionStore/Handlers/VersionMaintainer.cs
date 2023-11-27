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
                // Update model
                // Update the existing version model
                UpdateVersion<VersionEntityModel> updateVersion = new(_logger);
                var updatedVersionModel = updateVersion.Update(updatedEntity, existingModel);

                // Save the updated model
                await _versionStoreRepository.UpdateAsync(updatedVersionModel).ConfigureAwait(false);

            }


        }

    }
}
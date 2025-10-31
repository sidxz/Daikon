using Daikon.Events.MLogix;
using Daikon.Shared.Constants.MLogix;
using MLogix.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MLogix.Application.EventHandlers
{
    public partial class MLogixEventHandler : IMLogixEventHandler
    {

        public async Task OnEvent(MoleculeNuisancePredictedEvent @event)
        {
            MoleculePredictions moleculePredictions;
            try
            {
                // Get existing prediction
                moleculePredictions = await _moleculePredictionRepository.GetByMoleculeIdAsync(@event.MoleculeId);
                moleculePredictions ??= new MoleculePredictions
                {
                    MoleculeId = @event.MoleculeId,
                    NuisanceRequestStatus = NuisanceStatus.Pending,
                    NuisanceModelPredictions = [],
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MoleculeNuisancePredictedEvent for MoleculeId: {MoleculeId}", @event.MoleculeId);
                throw;
            }

            if (moleculePredictions == null)
            {
                _logger.LogError("MoleculePredictions is null for MoleculeId: {MoleculeId}", @event.MoleculeId);
                throw new Exception("MoleculePredictions cannot be null");
            }

            moleculePredictions.NuisanceRequestStatus = NuisanceStatus.Completed;

            // find if the ModelName already exists, update it, else add new
            var existingNuisance = moleculePredictions.NuisanceModelPredictions
                .FirstOrDefault(n => n.ModelName == @event.NuisancePrediction.ModelName);

            if (existingNuisance != null)
            {
                _mapper.Map(@event.NuisancePrediction, existingNuisance);
            }
            else
            {
                moleculePredictions.NuisanceModelPredictions.Add(@event.NuisancePrediction);
            }

            try
            {
                await _moleculePredictionRepository.UpsertAsync(moleculePredictions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating MoleculePredictions for MoleculeId: {MoleculeId}", @event.MoleculeId);
                throw;
            }

        }
    }
}
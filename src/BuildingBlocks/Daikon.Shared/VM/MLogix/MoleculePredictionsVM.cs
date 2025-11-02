using CQRS.Core.Domain;
using Daikon.Shared.Constants.MLogix;
using Daikon.Shared.Embedded.MLogix;

namespace Daikon.Shared.VM.MLogix
{
    public class MoleculePredictionsVM : VMMeta
    {
        public Guid MoleculeId { get; set; }

        /* Nuisance Predictions */
        public NuisanceStatus NuisanceRequestStatus { get; set; } = NuisanceStatus.NotRequested;
        public List<Nuisance> NuisanceModelPredictions { get; set; } = [];
        public List<Nuisance> HumanVerifiedNuisance { get; set; } = [];
    }
}
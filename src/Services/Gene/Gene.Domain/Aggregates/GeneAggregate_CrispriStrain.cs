
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, CrispriStrain> _crispriStrain = [];

        /* Add CrispriStrain */
        public void AddCrispriStrain(GeneCrispriStrainAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.CrispriStrainId == Guid.Empty)
            {
                throw new InvalidOperationException("CrispriStrain Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(@event.CrispriStrainName))
            {
                throw new InvalidOperationException($" The value of crispriStrainName cannot be null or whitespace");
            }
            if (_crispriStrain.ContainsKey(@event.CrispriStrainId))
            {
                throw new Exception("Expansion Props already exists.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneCrispriStrainAddedEvent @event)
        {
            _crispriStrain.Add(@event.CrispriStrainId, new CrispriStrain
            {
                CrispriStrainId = @event.CrispriStrainId,
                CrispriStrainName = @event.CrispriStrainName,
                Notes = @event.Notes
            });
        }

        /* Update CrispriStrain */
        public void UpdateCrispriStrain(GeneCrispriStrainUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_crispriStrain.ContainsKey(@event.CrispriStrainId))
            {
                throw new InvalidOperationException("CrispriStrain does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.CrispriStrainName))
            {
                throw new InvalidOperationException($" The value of crispriStrain classification cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneCrispriStrainUpdatedEvent @event)
        {
            _crispriStrain[@event.CrispriStrainId].CrispriStrainName = @event.CrispriStrainName;
            _crispriStrain[@event.CrispriStrainId].Notes = @event.Notes;
        }

        /* Delete CrispriStrain */
        public void DeleteCrispriStrain(GeneCrispriStrainDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_crispriStrain.ContainsKey(@event.CrispriStrainId))
            {
                throw new InvalidOperationException("CrispriStrain does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneCrispriStrainDeletedEvent @event)
        {
            _crispriStrain.Remove(@event.CrispriStrainId);
        }
    }
}
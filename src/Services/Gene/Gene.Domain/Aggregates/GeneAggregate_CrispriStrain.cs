
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, CrispriStrain> _crispriStrain = [];

        /* Add CrispriStrain */
        public void AddCrispriStrain(CrispriStrain crispriStrain)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(crispriStrain.CrispriStrainName))
            {
                throw new InvalidOperationException($" The value of crispriStrainName cannot be null or whitespace");
            }


            RaiseEvent(new GeneCrispriStrainAddedEvent
            {
                Id = _id,
                GeneId = _id,
                CrispriStrainId = crispriStrain.CrispriStrainId,
                CrispriStrainName = crispriStrain.CrispriStrainName,
                Notes = crispriStrain.Notes,
                DateCreated = DateTime.UtcNow
            });
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
        public void UpdateCrispriStrain(CrispriStrain crispriStrain)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_crispriStrain.ContainsKey(crispriStrain.CrispriStrainId))
            {
                throw new InvalidOperationException("CrispriStrain does not exist.");
            }
            if (string.IsNullOrWhiteSpace(crispriStrain.CrispriStrainName))
            {
                throw new InvalidOperationException($" The value of crispriStrain classification cannot be null or whitespace");
            }

            RaiseEvent(new GeneCrispriStrainUpdatedEvent
            {
                Id = _id,
                GeneId = _id,
                CrispriStrainId = crispriStrain.CrispriStrainId,
                CrispriStrainName = crispriStrain.CrispriStrainName,
                Notes = crispriStrain.Notes,
                DateUpdated = DateTime.UtcNow
            });
        }

        public void Apply(GeneCrispriStrainUpdatedEvent @event)
        {
            _id = @event.Id;
            _crispriStrain[@event.CrispriStrainId] = new CrispriStrain
            {
                CrispriStrainId = @event.CrispriStrainId,
                CrispriStrainName = @event.CrispriStrainName,
                Notes = @event.Notes
            };
        }

        /* Delete CrispriStrain */
        public void DeleteCrispriStrain(CrispriStrain crispriStrain)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_crispriStrain.ContainsKey(crispriStrain.CrispriStrainId))
            {
                throw new InvalidOperationException("CrispriStrain does not exist.");
            }

            RaiseEvent(new GeneCrispriStrainDeletedEvent
            {
                Id = _id,
                GeneId = _id,
                CrispriStrainId = crispriStrain.CrispriStrainId

            });
        }

        public void Apply(GeneCrispriStrainDeletedEvent @event)
        {
            _id = @event.Id;
            _crispriStrain.Remove(@event.CrispriStrainId);
        }
    }
}
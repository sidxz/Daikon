
using CQRS.Core.Domain;
using Daikon.Events.HitAssessment;
using HitAssessment.Domain.Entities;

namespace HitAssessment.Domain.Aggregates
{
    public partial class HaAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, HaCompoundEvolution> _compoundEvolutions = [];

        public void AddCompoundEvolution(HaCompoundEvolutionAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.CompoundEvolutionId == Guid.Empty)
            {
                throw new InvalidOperationException("Compound Evolution Id cannot be empty.");
            }

            if (_compoundEvolutions.ContainsKey(@event.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution already exists.");
            }
            RaiseEvent(@event);
        }

        public void Apply(HaCompoundEvolutionAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions.Add(@event.CompoundEvolutionId, new HaCompoundEvolution()
            {
                HitAssessmentId = @event.Id,
                MoleculeId = @event.MoleculeId,
                MIC = @event.MIC,
                IC50 = @event.IC50,
                IC50Unit = @event.IC50Unit,
                MICUnit = @event.MICUnit,
                Stage = @event.Stage,
                EvolutionDate = @event.EvolutionDate,
                Notes = @event.Notes

            });
        }

        public void UpdateCompoundEvolution(HaCompoundEvolutionUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.CompoundEvolutionId == Guid.Empty)
            {
                throw new InvalidOperationException("Compound Evolution Id cannot be empty.");
            }

            if (!_compoundEvolutions.ContainsKey(@event.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(HaCompoundEvolutionUpdatedEvent @event)
        {
            // Update the existing CompoundEvolution identified by @event.CompoundEvolutionId without creating a new one
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions[@event.CompoundEvolutionId].EvolutionDate = @event.EvolutionDate;
            _compoundEvolutions[@event.CompoundEvolutionId].Stage = @event.Stage;
            _compoundEvolutions[@event.CompoundEvolutionId].Notes = @event.Notes;
            _compoundEvolutions[@event.CompoundEvolutionId].MIC = @event.MIC;
            _compoundEvolutions[@event.CompoundEvolutionId].MICUnit = @event.MICUnit;
            _compoundEvolutions[@event.CompoundEvolutionId].IC50 = @event.IC50;
            _compoundEvolutions[@event.CompoundEvolutionId].IC50Unit = @event.IC50Unit;
        }

        public void DeleteCompoundEvolution(HaCompoundEvolutionDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }
            if (@event.CompoundEvolutionId == Guid.Empty)
            {
                throw new InvalidOperationException("Compound Evolution Id cannot be empty.");
            }

            if (!_compoundEvolutions.ContainsKey(@event.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(HaCompoundEvolutionDeletedEvent @event)
        {
            _compoundEvolutions.Remove(@event.CompoundEvolutionId);
        }
    }
}

using CQRS.Core.Domain;
using Daikon.Events.HitAssessment;
using HitAssessment.Domain.Entities;

namespace HitAssessment.Domain.Aggregates
{
    public partial class HaAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, HaCompoundEvolution> _compoundEvolutions = [];

        public void AddCompoundEvolution(HaCompoundEvolutionAddedEvent haCompoundEvolutionAddedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }

            if (_compoundEvolutions.ContainsKey(haCompoundEvolutionAddedEvent.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution already exists.");
            }
            RaiseEvent(haCompoundEvolutionAddedEvent);
        }

        public void Apply(HaCompoundEvolutionAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions.Add(@event.CompoundEvolutionId, new HaCompoundEvolution()
            {
                HitAssessmentId = @event.Id,
                CompoundId = @event.CompoundId,
                MIC = @event.MIC,
                IC50 = @event.IC50,
            });
        }

        public void UpdateCompoundEvolution(HaCompoundEvolutionUpdatedEvent haCompoundEvolutionUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }

            if (!_compoundEvolutions.ContainsKey(haCompoundEvolutionUpdatedEvent.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution does not exist.");
            }

            RaiseEvent(haCompoundEvolutionUpdatedEvent);
        }

        public void Apply(HaCompoundEvolutionUpdatedEvent @event)
        {
            // Update the existing CompoundEvolution identified by @event.CompoundEvolutionId without creating a new one
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions[@event.CompoundEvolutionId].MIC = @event.MIC;
            _compoundEvolutions[@event.CompoundEvolutionId].IC50 = @event.IC50;
        }

        public void DeleteCompoundEvolution(HaCompoundEvolutionDeletedEvent haCompoundEvolutionDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }

            if (!_compoundEvolutions.ContainsKey(haCompoundEvolutionDeletedEvent.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution does not exist.");
            }

            RaiseEvent(haCompoundEvolutionDeletedEvent);
        }

        public void Apply(HaCompoundEvolutionDeletedEvent @event)
        {
            _compoundEvolutions.Remove(@event.CompoundEvolutionId);
        }
    }
}
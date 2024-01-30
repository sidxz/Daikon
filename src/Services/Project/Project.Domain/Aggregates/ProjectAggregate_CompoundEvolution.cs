
using CQRS.Core.Domain;
using Daikon.Events.Project;
using Project.Domain.Entities;

namespace Project.Domain.Aggregates
{
    public partial class ProjectAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, ProjectCompoundEvolution> _compoundEvolutions = [];

        public void AddCompoundEvolution(ProjectCompoundEvolutionAddedEvent ProjectCompoundEvolutionAddedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            if (_compoundEvolutions.ContainsKey(ProjectCompoundEvolutionAddedEvent.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution already exists.");
            }
            RaiseEvent(ProjectCompoundEvolutionAddedEvent);
        }

        public void Apply(ProjectCompoundEvolutionAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions.Add(@event.CompoundEvolutionId, new ProjectCompoundEvolution()
            {
                ProjectId = @event.Id,
                CompoundId = @event.CompoundId,
                MIC = @event.MIC,
                IC50 = @event.IC50,
            });
        }

        public void UpdateCompoundEvolution(ProjectCompoundEvolutionUpdatedEvent ProjectCompoundEvolutionUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            if (!_compoundEvolutions.ContainsKey(ProjectCompoundEvolutionUpdatedEvent.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution does not exist.");
            }

            RaiseEvent(ProjectCompoundEvolutionUpdatedEvent);
        }

        public void Apply(ProjectCompoundEvolutionUpdatedEvent @event)
        {
            // Update the existing CompoundEvolution identified by @event.CompoundEvolutionId without creating a new one
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions[@event.CompoundEvolutionId].MIC = @event.MIC;
            _compoundEvolutions[@event.CompoundEvolutionId].IC50 = @event.IC50;
        }

        public void DeleteCompoundEvolution(ProjectCompoundEvolutionDeletedEvent ProjectCompoundEvolutionDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            if (!_compoundEvolutions.ContainsKey(ProjectCompoundEvolutionDeletedEvent.CompoundEvolutionId))
            {
                throw new Exception("Compound Evolution does not exist.");
            }

            RaiseEvent(ProjectCompoundEvolutionDeletedEvent);
        }

        public void Apply(ProjectCompoundEvolutionDeletedEvent @event)
        {
            _compoundEvolutions.Remove(@event.CompoundEvolutionId);
        }
    }
}
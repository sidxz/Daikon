
using CQRS.Core.Domain;
using Daikon.Events.Project;
using Project.Domain.Entities;

namespace Project.Domain.Aggregates
{
    public partial class ProjectAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, ProjectCompoundEvolution> _compoundEvolutions = [];

        public void AddCompoundEvolution(ProjectCompoundEvolutionAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
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

        public void Apply(ProjectCompoundEvolutionAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _compoundEvolutions.Add(@event.CompoundEvolutionId, new ProjectCompoundEvolution()
            {
                ProjectId = @event.Id,
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

        public void UpdateCompoundEvolution(ProjectCompoundEvolutionUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
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

        public void Apply(ProjectCompoundEvolutionUpdatedEvent @event)
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

        public void DeleteCompoundEvolution(ProjectCompoundEvolutionDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
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

        public void Apply(ProjectCompoundEvolutionDeletedEvent @event)
        {
            _compoundEvolutions.Remove(@event.CompoundEvolutionId);
        }
    }
}
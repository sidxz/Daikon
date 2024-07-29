using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Events.Targets;
using Target.Domain.Entities;

namespace Target.Domain.Aggregates
{
    public partial class TargetAggregate
    {

        public readonly Dictionary<Guid, Toxicology> _toxicology = [];

        public void AddToxicology(TargetToxicologyAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ToxicologyId == Guid.Empty)
            {
                throw new InvalidOperationException("Toxicology Id cannot be empty.");
            }
            if (_toxicology.ContainsKey(@event.ToxicologyId))
            {
                throw new Exception("Toxicology already exists.");
            }
            if (string.IsNullOrWhiteSpace(@event.Topic))
            {
                throw new InvalidOperationException($" The value of topic cannot be null or whitespace");
            }
            // allow one topic name per target
            if (_toxicology.Any(x => x.Value.Topic == @event.Topic))
            {
                throw new InvalidOperationException($"The topic name '{@event.Topic}' already exists.");
            }
            RaiseEvent(@event);
        }

        public void Apply(TargetToxicologyAddedEvent @event)
        {
            _toxicology.Add(@event.ToxicologyId, new Toxicology
            {
                ToxicologyId = @event.ToxicologyId,
                Topic = @event.Topic,
                Impact = @event.Impact,
                ImpactPriority = @event.ImpactPriority,
                Likelihood = @event.Likelihood,
                LikelihoodPriority = @event.LikelihoodPriority,
                Note = @event.Note
            });
        }


        public void UpdateToxicology(TargetToxicologyUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ToxicologyId == Guid.Empty)
            {
                throw new InvalidOperationException("Toxicology Id cannot be empty.");
            }
            if (!_toxicology.ContainsKey(@event.ToxicologyId))
            {
                throw new Exception("Toxicology does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.Topic))
            {
                throw new InvalidOperationException($" The value of topic cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }

        public void Apply(TargetToxicologyUpdatedEvent @event)
        {
            _toxicology[@event.ToxicologyId].Topic = @event.Topic;
            _toxicology[@event.ToxicologyId].Impact = @event.Impact;
            _toxicology[@event.ToxicologyId].ImpactPriority = @event.ImpactPriority;
            _toxicology[@event.ToxicologyId].Likelihood = @event.Likelihood;
            _toxicology[@event.ToxicologyId].LikelihoodPriority = @event.LikelihoodPriority;
            _toxicology[@event.ToxicologyId].Note = @event.Note;
        }


        public void DeleteToxicology(TargetToxicologyDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ToxicologyId == Guid.Empty)
            {
                throw new InvalidOperationException("Toxicology Id cannot be empty.");
            }
            if (!_toxicology.ContainsKey(@event.ToxicologyId))
            {
                throw new Exception("Toxicology does not exist.");
            }
            RaiseEvent(@event);
        }

        public void Apply(TargetToxicologyDeletedEvent @event)
        {
            _toxicology.Remove(@event.ToxicologyId);
        }

    }
}
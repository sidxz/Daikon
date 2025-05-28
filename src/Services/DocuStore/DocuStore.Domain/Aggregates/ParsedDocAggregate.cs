
using Daikon.Events.DocuStore;
using Daikon.EventStore.Aggregate;

namespace DocuStore.Domain.Aggregates
{
    public partial class ParsedDocAggregate : AggregateRoot
    {
        private bool _active;
        private string _name;
        public ParsedDocAggregate()
        {

        }

        public ParsedDocAggregate(ParsedDocAddedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Id cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(@event.Name))
            {
                throw new InvalidOperationException("Name cannot be null or whitespace.");
            }

            _active = true;
            _id = @event.Id;
            _name = @event.Name;

            RaiseEvent(@event);

        }

        public void Apply(ParsedDocAddedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _name = @event.Name;
        }

        public void UpdateParsedDoc(ParsedDocUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This ParsedDoc is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Id cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(@event.Name))
            {
                throw new InvalidOperationException("Name cannot be null or whitespace.");
            }

            _id = @event.Id;
            _name = @event.Name;
            RaiseEvent(@event);
        }

        public void Apply(ParsedDocUpdatedEvent @event)
        {
            _id = @event.Id;
            _name = @event.Name;
        }

        public void DeleteParsedDoc(ParsedDocDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This ParsedDoc is already deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Id cannot be empty.");
            }
            _active = false;
            RaiseEvent(@event);
        }

        public void Apply(ParsedDocDeletedEvent @event)
        {
            _active = false;
        }
    }
}

using CQRS.Core.Event;

namespace Daikon.Events.MLogix
{
    public class MoleculeDeletedEvent : BaseEvent
    {
        public MoleculeDeletedEvent() : base(nameof(MoleculeDeletedEvent))
        {

        }
        
    }
    
}
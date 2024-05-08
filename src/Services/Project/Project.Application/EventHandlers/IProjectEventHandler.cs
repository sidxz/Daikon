
using Daikon.Events.Project;

namespace Project.Application.EventHandlers
{
    public interface IProjectEventHandler
    {
        Task OnEvent(ProjectCreatedEvent @event);
        Task OnEvent(ProjectUpdatedEvent @event);
        Task OnEvent(ProjectDeletedEvent @event);
        Task OnEvent(ProjectRenamedEvent @event);

        Task OnEvent(ProjectAssociationUpdatedEvent @event);

        Task OnEvent(ProjectCompoundEvolutionAddedEvent @event);
        Task OnEvent(ProjectCompoundEvolutionUpdatedEvent @event);
        Task OnEvent(ProjectCompoundEvolutionDeletedEvent @event);


    }
}
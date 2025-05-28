using System;
using System.Threading.Tasks;
using Daikon.Events.HitAssessment;
using Daikon.Events.Project;
using Daikon.Shared.Constants.AppProject;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleProjectCreatedEvent(ProjectCreatedEvent @event)
        {
            try
            {
                var organizationName = await GetOrganizationNameAsync(@event.PrimaryOrgId);
                var createdByUser = await GetUserNameAsync(@event.CreatedById);
                return new EventMessageResult
                {
                    Message = $"<b>{organizationName}</b> added a new Project <b>{@event.Name}</b>, created by {createdByUser}",
                    Link = GetProjectLink(@event.Stage, @event.Id),
                    EventType = nameof(ProjectCreatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ProjectCreatedEvent");
                throw;
            }
        }

        private async Task<EventMessageResult> HandleProjectUpdatedEvent(ProjectUpdatedEvent @event)
        {
            string projectName = "Unknown Project";
            string updatedByUser = "Unknown User";
            string organizationName = "Unknown Organization";

            try
            {
                var project = await _projectAPI.GetById(@event.Id, this.refreshCache);
                if (project != null)
                {
                    projectName = project.Name;
                    organizationName = await GetOrganizationNameAsync(project.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning($"Project not found: {@event.Id}");
                }

                if (@event.LastModifiedById != Guid.Empty)
                {
                    updatedByUser = await GetUserNameAsync(@event.LastModifiedById);
                }

                return new EventMessageResult
                {
                    Message = $"Project <b>{projectName} ({organizationName})</b> was updated by {updatedByUser}",
                    Link = GetProjectLink(@event.Stage, @event.Id),
                    EventType = nameof(ProjectUpdatedEvent)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process ProjectUpdatedEvent for Id: {@event.Id}");
                throw;
            }
        }


        private async Task<EventMessageResult> HandleProjectCEAddedEvent(ProjectCompoundEvolutionAddedEvent @event)
        {
            return await HandleProjectCompoundEvolutionEvent(@event, "A new Compound Evolution was added to the Project");
        }

        private async Task<EventMessageResult> HandleProjectCEUpdatedEvent(ProjectCompoundEvolutionUpdatedEvent @event)
        {
            return await HandleProjectCompoundEvolutionEvent(@event, "Compound Evolution of Project was modified");
        }

        private async Task<EventMessageResult> HandleProjectCompoundEvolutionEvent(dynamic @event, string actionMessage)
        {
            string projectName = "Unknown Project";
            string updatedByUser = "Unknown User";
            string organizationName = "Unknown Organization";

            try
            {
                var project = await _projectAPI.GetById(@event.Id, this.refreshCache);
                if (project != null)
                {
                    projectName = project.Name;
                    organizationName = await GetOrganizationNameAsync(project.PrimaryOrgId);
                }
                else
                {
                    _logger.LogWarning($"Project not found: {@event.Id}");
                }

                if (@event.LastModifiedById != Guid.Empty)
                {
                    updatedByUser = await GetUserNameAsync(@event.LastModifiedById);
                }

                return new EventMessageResult
                {
                    Message = $"{actionMessage} <b>{projectName} ({organizationName})</b> by {updatedByUser}",
                    Link = GetProjectLink(project.Stage, project.Id),
                    EventType = @event.GetType().Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process {@event.GetType().Name} for Id: {@event.Id}");
                throw;
            }
        }

        private string GetProjectLink(string stage, Guid projectId)
        {
            if (stage == ProjectStage.H2L || stage == ProjectStage.LO || stage == ProjectStage.SP)
            {
                return $"/wf/portfolio/viewer/{projectId}";
            }
            else    
            {
                return $"/wf/post-portfolio/viewer/{projectId}";
            }
        }
    }
}

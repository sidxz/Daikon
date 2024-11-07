using System;
using System.Threading.Tasks;
using Daikon.Events.Gene;
using Microsoft.Extensions.Logging;

namespace EventHistory.Application.Features.Processors
{
    public partial class EventMessageProcessor
    {
        private async Task<EventMessageResult> HandleGeneCreatedEventAsync(GeneCreatedEvent createdEvent)
        {
            return new EventMessageResult
            {
                Message = $"A new gene <b>{createdEvent.Name}</b> ({createdEvent.AccessionNumber}) has been added.",
                Link = $"/wf/gene/{createdEvent.Id}",
                EventType = nameof(GeneCreatedEvent)
            };
        }

        private async Task<EventMessageResult> HandleGeneUpdatedEventAsync(GeneUpdatedEvent updatedEvent)
        {
            string geneName = await GetGeneNameAsync(updatedEvent.Id);
            string updatedByUser = await GetUserNameAsync(updatedEvent.LastModifiedById) ?? "Unknown User";
            return new EventMessageResult
            {
                Message = $"Gene <b>{geneName}</b> was updated by {updatedByUser}",
                Link = $"/wf/gene/{updatedEvent.Id}",
                EventType = nameof(GeneUpdatedEvent)
            };
        }

        private async Task<EventMessageResult> HandleGeneCrispriStrainAddedEventAsync(GeneCrispriStrainAddedEvent crispriEvent)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "A CRISPRi Strain has been added to gene <b>{0}</b>, added by {1}",
                crispriEvent.GeneId,
                crispriEvent.CreatedById,
                nameof(GeneCrispriStrainAddedEvent),
                "/private");
        }

        private async Task<EventMessageResult> HandleGeneCrispriStrainUpdatedEventAsync(GeneCrispriStrainUpdatedEvent crispriUpdateEvent)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "CRISPRi Strain of gene <b>{0}</b> has been updated by {1}",
                crispriUpdateEvent.GeneId,
                crispriUpdateEvent.LastModifiedById,
                nameof(GeneCrispriStrainUpdatedEvent),
                "/private");
        }

        private async Task<EventMessageResult> HandleGeneEssentialityAddedEventAsync(GeneEssentialityAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Gene Essentiality has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneEssentialityAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneEssentialityUpdatedEventAsync(GeneEssentialityUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Gene Essentiality of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneEssentialityUpdatedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneHypomorphAddedEventAsync(GeneHypomorphAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Hypomorph has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneHypomorphAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneHypomorphUpdatedEventAsync(GeneHypomorphUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Hypomorph of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneHypomorphUpdatedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneProteinActivityAssayAddedEventAsync(GeneProteinActivityAssayAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Protein Activity Assay has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneProteinActivityAssayAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneProteinActivityAssayUpdatedEventAsync(GeneProteinActivityAssayUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Protein Activity Assay of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneProteinActivityAssayUpdatedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneProteinProductionAddedEventAsync(GeneProteinProductionAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Protein Production has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneProteinProductionAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneProteinProductionUpdatedEventAsync(GeneProteinProductionUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Protein Production of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneProteinProductionUpdatedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneResistanceMutationAddedEventAsync(GeneResistanceMutationAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Resistance Mutation has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneResistanceMutationAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneResistanceMutationUpdatedEventAsync(GeneResistanceMutationUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Resistance Mutation of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneResistanceMutationUpdatedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneUnpublishedStructuralInformationAddedEventAsync(GeneUnpublishedStructuralInformationAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Unpublished Structural Information has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneUnpublishedStructuralInformationAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneUnpublishedStructuralInformationUpdatedEventAsync(GeneUnpublishedStructuralInformationUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Unpublished Structural Information of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneUnpublishedStructuralInformationUpdatedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneVulnerabilityAddedEventAsync(GeneVulnerabilityAddedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Vulnerability has been added for gene <b>{0}</b> by {1}",
                eventInstance.GeneId,
                eventInstance.CreatedById,
                nameof(GeneVulnerabilityAddedEvent),
                "/private");
        }

        
        private async Task<EventMessageResult> HandleGeneVulnerabilityUpdatedEventAsync(GeneVulnerabilityUpdatedEvent eventInstance)
        {
            return await CreateGeneRelatedEventMessageAsync(
                "Vulnerability of gene <b>{0}</b> has been updated by {1}",
                eventInstance.GeneId,
                eventInstance.LastModifiedById,
                nameof(GeneVulnerabilityUpdatedEvent),
                "/private");
        }

        
        // Helper function to streamline event message creation for gene-related events
        private async Task<EventMessageResult> CreateGeneRelatedEventMessageAsync(
            string messageTemplate,
            Guid geneId,
            Guid? userId,
            string eventType,
            string linkSuffix = "")
        {
            try
            {
                string geneName = await GetGeneNameAsync(geneId);
                string userName = await GetUserNameAsync(userId) ?? "Unknown User";
                string link = $"/wf/gene/{geneId}{linkSuffix}";

                return new EventMessageResult
                {
                    Message = string.Format(messageTemplate, geneName, userName),
                    Link = link,
                    EventType = eventType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling {eventType}");
                throw;
            }
        }

        // Helper function to get gene name with null handling and logging
        private async Task<string> GetGeneNameAsync(Guid geneId)
        {
            var gene = await _geneAPI.GetBasicById(geneId);
            if (gene == null)
            {
                _logger.LogWarning($"Gene not found: {geneId}");
                return "Unknown Gene";
            }
            return gene.Name;
        }        
    }
}

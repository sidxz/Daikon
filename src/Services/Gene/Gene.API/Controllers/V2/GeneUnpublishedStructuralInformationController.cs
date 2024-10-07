
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteUnpublishedStructuralInformation;
using Gene.Application.Features.Command.NewUnpublishedStructuralInformation;
using Gene.Application.Features.Command.UpdateUnpublishedStructuralInformation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {


        [HttpPost("{id}/unpublished-structural-information", Name = "AddUnpublishedStructuralInformation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddUnpublishedStructuralInformation(Guid id, NewUnpublishedStructuralInformationCommand command)
        {
            var unpublishedStructuralInformationId = Guid.NewGuid();
            command.UnpublishedStructuralInformationId = unpublishedStructuralInformationId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = unpublishedStructuralInformationId,
                Message = "Unpublished structural information added successfully",
            });
        }


        [HttpPut("{id}/unpublished-structural-information/{unpublishedStructuralInformationId}", Name = "UpdateUnpublishedStructuralInformation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateUnpublishedStructuralInformation(Guid id, Guid unpublishedStructuralInformationId, UpdateUnpublishedStructuralInformationCommand command)
        {
            command.Id = id;
            command.UnpublishedStructuralInformationId = unpublishedStructuralInformationId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Unpublished structural information updated successfully",
            });

        }

        [HttpDelete("{id}/unpublished-structural-information/{unpublishedStructuralInformationId}", Name = "DeleteUnpublishedStructuralInformation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUnpublishedStructuralInformation(Guid id, Guid unpublishedStructuralInformationId)
        {
            await _mediator.Send(new DeleteUnpublishedStructuralInformationCommand { Id = id, GeneId = id, UnpublishedStructuralInformationId = unpublishedStructuralInformationId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Unpublished structural information deleted successfully",
            });
        }
    }
}
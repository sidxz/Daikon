
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteResistanceMutation;
using Gene.Application.Features.Command.NewResistanceMutation;
using Gene.Application.Features.Command.UpdateResistanceMutation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {

        [HttpPost("{id}/resistance-mutation", Name = "AddResistanceMutation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddResistanceMutation(Guid id, NewResistanceMutationCommand command)
        {
            var resistanceMutationId = Guid.NewGuid();
            command.ResistanceMutationId = resistanceMutationId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = resistanceMutationId,
                Message = "Resistance mutation added successfully",
            });

        }

        [HttpPut("{id}/resistance-mutation/{resistanceMutationId}", Name = "UpdateResistanceMutation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateResistanceMutation(Guid id, Guid resistanceMutationId, UpdateResistanceMutationCommand command)
        {
            command.Id = id;
            command.ResistanceMutationId = resistanceMutationId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Resistance mutation updated successfully",
            });

        }

        [HttpDelete("{id}/resistance-mutation/{resistanceMutationId}", Name = "DeleteResistanceMutation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteResistanceMutation(Guid id, Guid resistanceMutationId)
        {
            await _mediator.Send(new DeleteResistanceMutationCommand { Id = id, GeneId = id, ResistanceMutationId = resistanceMutationId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Resistance mutation deleted successfully",
            });

        }
    }
}
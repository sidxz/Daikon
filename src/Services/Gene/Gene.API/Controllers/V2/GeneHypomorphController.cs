
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteHypomorph;
using Gene.Application.Features.Command.NewHypomorph;
using Gene.Application.Features.Command.UpdateHypomorph;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {
        [HttpPost("{id}/hypomorph", Name = "AddHypomorph")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddHypomorph(Guid id, NewHypomorphCommand command)
        {
            var hypomorphId = Guid.NewGuid();
            command.HypomorphId = hypomorphId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = hypomorphId,
                Message = "Hypomorph added successfully",
            });
        }

        [HttpPut("{id}/hypomorph/{hypomorphId}", Name = "UpdateHypomorph")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateHypomorph(Guid id, Guid hypomorphId, UpdateHypomorphCommand command)
        {
            command.Id = id;
            command.HypomorphId = hypomorphId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hypomorph updated successfully",
            });
        }

        [HttpDelete("{id}/hypomorph/{hypomorphId}", Name = "DeleteHypomorph")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHypomorph(Guid id, Guid hypomorphId)
        {
            await _mediator.Send(new DeleteHypomorphCommand { Id = id, GeneId = id, HypomorphId = hypomorphId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hypomorph deleted successfully",
            });
        }
    }
}
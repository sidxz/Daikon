
using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteEssentiality;
using Gene.Application.Features.Command.NewEssentiality;
using Gene.Application.Features.Command.UpdateEssentiality;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {

        [HttpPost("{id}/essentiality", Name = "AddEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddEssentiality(Guid id, NewEssentialityCommand command)
        {
            var essentialityId = Guid.NewGuid();
            command.EssentialityId = essentialityId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = essentialityId,
                Message = "Essentiality added successfully",
            });

        }

        [HttpPut("{id}/essentiality/{essentialityId}", Name = "UpdateEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEssentiality(Guid id, Guid essentialityId, UpdateEssentialityCommand command)
        {
            command.Id = id;
            command.EssentialityId = essentialityId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Essentiality updated successfully",
            });

        }

        [HttpDelete("{id}/essentiality/{essentialityId}", Name = "DeleteEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEssentiality(Guid id, Guid essentialityId)
        {
            await _mediator.Send(new DeleteEssentialityCommand { Id = id, GeneId = id, EssentialityId = essentialityId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Essentiality deleted successfully",
            });

        }
    }
}
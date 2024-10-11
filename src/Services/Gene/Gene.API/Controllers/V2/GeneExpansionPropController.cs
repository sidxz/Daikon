using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.AddExpansionProp;
using Gene.Application.Features.Command.DeleteExpansionProp;
using Gene.Application.Features.Command.UpdateExpansionProp;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    public partial class GeneController : ControllerBase
    {
        [HttpPost("{id}/expansion-prop", Name = "AddExpansionProp")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddExpansionProp(Guid id, AddExpansionPropCommand command)
        {
            var expansionPropId = Guid.NewGuid();
            command.Id = id;
            command.ExpansionPropId = expansionPropId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = expansionPropId,
                Message = "Expansion property added successfully",
            });
        }

        [HttpPut("{id}/expansion-prop/{expansionPropId}", Name = "UpdateExpansionProp")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateExpansionProp(Guid id, Guid expansionPropId, UpdateExpansionPropCommand command)
        {
            command.Id = id;
            command.ExpansionPropId = expansionPropId;

            await _mediator.Send(command);

            return Ok(new BaseResponse
            {
                Message = "Expansion property updated successfully"
            });
        }

        [HttpDelete("{id}/expansion-prop/{expansionPropId}", Name = "DeleteExpansionProp")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteExpansionProp(Guid id, Guid expansionPropId)
        {
            var command = new DeleteExpansionPropCommand
            {
                Id = id,
                ExpansionPropId = expansionPropId
            };

            await _mediator.Send(command);

            return Ok(new BaseResponse
            {
                Message = "Expansion property deleted successfully"
            });

        }
    }
}

using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteProteinProduction;
using Gene.Application.Features.Command.NewProteinProduction;
using Gene.Application.Features.Command.UpdateProteinProduction;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {

        [HttpPost("{id}/protein-production", Name = "AddProteinProduction")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddProteinProduction(Guid id, NewProteinProductionCommand command)
        {
            var proteinProductionId = Guid.NewGuid();
            command.ProteinProductionId = proteinProductionId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = proteinProductionId,
                Message = "Protein production added successfully",
            });
        }

        [HttpPut("{id}/protein-production/{proteinProductionId}", Name = "UpdateProteinProduction")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProteinProduction(Guid id, Guid proteinProductionId, UpdateProteinProductionCommand command)
        {
            command.Id = id;
            command.ProteinProductionId = proteinProductionId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Protein production updated successfully",
            });

        }

        [HttpDelete("{id}/protein-production/{proteinProductionId}", Name = "DeleteProteinProduction")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProteinProduction(Guid id, Guid proteinProductionId)
        {
            await _mediator.Send(new DeleteProteinProductionCommand { Id = id, GeneId = id, ProteinProductionId = proteinProductionId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Protein production deleted successfully",
            });
        }
    }
}
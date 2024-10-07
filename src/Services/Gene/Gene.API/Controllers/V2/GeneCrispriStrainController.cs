
using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteCrispriStrain;
using Gene.Application.Features.Command.NewCrispriStrain;
using Gene.Application.Features.Command.UpdateCrispriStrain;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {

        [HttpPost("{id}/crispri-strain", Name = "AddCrispriStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddCrispriStrain(Guid id, NewCrispriStrainCommand command)
        {
            var crispriStrainId = Guid.NewGuid();
            command.CrispriStrainId = crispriStrainId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = crispriStrainId,
                Message = "CRISPRi Strain added successfully",
            });
        }



        [HttpPut("{id}/crispri-strain/{crispriStrainId}", Name = "UpdateCrispriStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCrispriStrain(Guid id, Guid crispriStrainId, UpdateCrispriStrainCommand command)
        {
            command.Id = id;
            command.CrispriStrainId = crispriStrainId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "CRISPRi Strain updated successfully",
            });

        }

        [HttpDelete("{id}/crispri-strain/{crispriStrainId}", Name = "DeleteCrispriStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCrispriStrain(Guid id, Guid crispriStrainId)
        {
            await _mediator.Send(new DeleteCrispriStrainCommand { Id = id, GeneId = id, CrispriStrainId = crispriStrainId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "CRISPRi Strain deleted successfully",
            });
        }

    }
}

using System.Net;
using CQRS.Core.Responses;
using HitAssessment.Application.Features.Commands.DeleteHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.NewHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution;
using Microsoft.AspNetCore.Mvc;

namespace HitAssessment.API.Controllers.V2
{
    public partial class HitAssessmentController : ControllerBase
    {
        [HttpPost("{haId}/compound-evolution/", Name = "AddCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddCompoundEvolution(Guid haId, NewHaCompoundEvolutionCommand command)
        {
            command.Id = haId;
            var compoundEvolutionId = Guid.NewGuid();
            command.CompoundEvolutionId = compoundEvolutionId;
            var response = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, response);
        }


        [HttpPut("{haId}/compound-evolution/{id}", Name = "UpdateCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCompoundEvolution(Guid haId, Guid id, UpdateHaCompoundEvolutionCommand command)
        {
            command.Id = haId;
            command.CompoundEvolutionId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "HA Compound Evolution updated successfully",
            });
        }

        [HttpDelete("{haId}/compound-evolution/{id}", Name = "DeleteCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCompoundEvolution(Guid haId, Guid id)
        {
            await _mediator.Send(new DeleteHaCompoundEvolutionCommand
            {
                Id = haId,
                CompoundEvolutionId = id
            });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "HA Compound Evolution deleted successfully",
            });
        }
    }
}
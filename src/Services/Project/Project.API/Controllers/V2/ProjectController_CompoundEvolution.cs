
using System.Net;
using CQRS.Core.Responses;
using Project.Application.Features.Commands.DeleteProjectCompoundEvolution;
using Project.Application.Features.Commands.NewProjectCompoundEvolution;
using Project.Application.Features.Commands.UpdateProjectCompoundEvolution;
using Microsoft.AspNetCore.Mvc;

namespace Project.API.Controllers.V2
{
    public partial class ProjectController : ControllerBase
    {
        [HttpPost("{projectId}/compound-evolution/", Name = "AddCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddCompoundEvolution(Guid projectId, NewProjectCompoundEvolutionCommand command)
        {
            command.Id = projectId;
            var compoundEvolutionId = Guid.NewGuid();
            command.CompoundEvolutionId = compoundEvolutionId;
            var response = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, response);

        }



        [HttpPut("{projectId}/compound-evolution/{id}", Name = "UpdateCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCompoundEvolution(Guid projectId, Guid id, UpdateProjectCompoundEvolutionCommand command)
        {
            command.Id = projectId;
            command.CompoundEvolutionId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project compound evolution updated successfully",
            });

        }


        [HttpDelete("{projectId}/compound-evolution/{id}", Name = "DeleteCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCompoundEvolution(Guid projectId, Guid id)
        {
            await _mediator.Send(new DeleteProjectCompoundEvolutionCommand
            {
                Id = projectId,
                CompoundEvolutionId = id
            });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project compound evolution deleted successfully",
            });

        }
    }
}
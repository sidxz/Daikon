
using System.Net;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Commands.DeleteScreenRun;
using Screen.Application.Features.Commands.NewScreenRun;
using Screen.Application.Features.Commands.UpdateScreenRun;

namespace Screen.API.Controllers.V2
{
    public partial class ScreenController : ControllerBase
    {
        [HttpPost("{screenId}/screen-run", Name = "AddScreenRun")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddScreenRun(Guid screenId, NewScreenRunCommand command)
        {
            command.Id = screenId;
            command.ScreenRunId = Guid.NewGuid();
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = command.ScreenRunId,
                Message = "Screen run added successfully",
            });
        }


        [HttpPut("{screenId}/screen-run/{screenRunId}", Name = "UpdateScreenRun")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateScreenRun(Guid screenId, Guid screenRunId, UpdateScreenRunCommand command)
        {
            command.Id = screenId;
            command.ScreenRunId = screenRunId;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen run updated successfully",
            });
        }


        [HttpDelete("{screenId}/screen-run/{screenRunId}", Name = "DeleteScreenRun")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteScreenRun(Guid screenId, Guid screenRunId)
        {
            await _mediator.Send(new DeleteScreenRunCommand { Id = screenId, ScreenRunId = screenRunId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen run deleted successfully",
            });
        }
    }
}
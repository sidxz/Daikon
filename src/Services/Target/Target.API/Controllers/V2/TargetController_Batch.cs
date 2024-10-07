
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Target.Application.BatchOperations.BatchCommands.ImportOne;

namespace Target.API.Controllers.V2
{
    public partial class TargetController : ControllerBase
    {
        [HttpPost("batch/import-one")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ImportOne([FromBody] ImportOneCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = command.Id == Guid.Empty ? id : command.Id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Target added successfully",
            });
        }
    }
}
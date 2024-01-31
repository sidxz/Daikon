
using Microsoft.AspNetCore.Mvc;
using UserStore.Application.Features.Commands.Users.ValidateUserAccess;

namespace UserStore.API.Controllers.V2
{
    public partial class UserController : ControllerBase
    {
        [HttpPost("validate")]
        [ProducesResponseType(typeof(ValidateUserAccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateUserAccess([FromBody] ValidateUserAccessCommand command)
        {
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid request.");
                return BadRequest(ex.Message);
            }
        }
    }
}
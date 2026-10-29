
using Microsoft.AspNetCore.Mvc;
using UserStore.Application.Features.Commands.Users.ResolvePermission;

namespace UserStore.API.Controllers.V2
{
    public partial class UserController : ControllerBase
    {
        [HttpPost("resolve-permission")]
        [ProducesResponseType(typeof(ResolvePermissionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResolvePermission([FromBody] ResolvePermissionQuery query)
        {
            try
            {
                var response = await _mediator.Send(query);
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
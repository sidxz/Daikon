
using CQRS.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserStore.Application.Features.Queries.AppVars.GetAppVars;
using UserStore.Application.Features.Queries.GlobalValues.GetGlobalValues;

namespace UserStore.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AppSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppSettingsController> _logger;

        public AppSettingsController(IMediator mediator, ILogger<AppSettingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("global-values")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGlobalValues()
        {
            try
            {
                var globalValues = await _mediator.Send(new GetGlobalValuesQuery());
                return Ok(globalValues);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to get global values: {e.Message}");
                return BadRequest($"Unable to get global values: {e.Message}");
            }
        }

        [HttpGet("app-vars")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAppVars()
        {
            var query = new GetAppVarsQuery { UserId = User.GetAppUserId() };

            try
            {
                var appVars = await _mediator.Send(query);
                return Ok(appVars);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to get app vars: {e.Message}");
                return BadRequest($"Unable to get app vars: {e.Message}");
            }
        }

    }
}
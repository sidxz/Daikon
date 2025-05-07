// ✅ TableCustomizationController.cs
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserPreferences.Application.Features.Commands.RemoveTableUserCustom;
using UserPreferences.Application.Features.Commands.SetTableDefaults;
using UserPreferences.Application.Features.Commands.SetTableGlobal;
using UserPreferences.Application.Features.Commands.SetTableUserCustom;
using UserPreferences.Application.Features.Queries.GetDefaults;
using UserPreferences.Application.Features.Queries.ResolveTableConfig;

namespace UserPreferences.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/user-preferences/[controller]")]
    [ApiVersion("2.0")]
    public class TableCustomizationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet("get-table-defaults")]
        [ProducesResponseType(typeof(List<Domain.Table.TableDefaults>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<Domain.Table.TableDefaults>>> GetDefaults([FromQuery] string tableType)
        {
            var query = new GetDefaultsQuery
            {
                TableType = tableType
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("set-table-defaults")]
        [ProducesResponseType(typeof(Domain.Table.TableDefaults), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Table.TableDefaults>> SetTableDefaults([FromBody] SetTableDefaultsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("set-table-global")] 
        [ProducesResponseType(typeof(Domain.Table.TableGlobalConfig), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Table.TableGlobalConfig>> SetTableGlobal([FromBody] SetTableGlobalConfigCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("set-user-customization")]
        [ProducesResponseType(typeof(Domain.Table.TableUserCustomization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Table.TableUserCustomization>> SetUserCustomization([FromBody] SetTableUserCustomizationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("remove-user-customization")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> RemoveUserCustomization([FromBody] RemoveTableUserCustomizationCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("resolve")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<string>>> ResolveTableConfig([FromQuery] string tableType, [FromQuery] Guid tableInstanceId, [FromQuery] Guid userId)
        {
            var query = new ResolveTableConfigQuery
            {
                TableType = tableType,
                TableInstanceId = tableInstanceId,
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

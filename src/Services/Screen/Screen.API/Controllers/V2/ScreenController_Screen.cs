
using System.Net;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Batch.ImportOne;
using Screen.Application.Features.Commands.DeleteScreen;
using Screen.Application.Features.Commands.NewScreen;
using Screen.Application.Features.Commands.RenameScreen;
using Screen.Application.Features.Commands.UpdateScreen;
using Screen.Application.Features.Commands.UpdateScreenAssociatedTargets;
using Screen.Application.Features.Queries.GetScreen.ById;
using Screen.Application.Features.Queries.GetScreen.ByName;
using Screen.Application.Features.Queries.GetScreensList;
using Daikon.Shared.VM.Screen;
namespace Screen.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class ScreenController(IMediator mediator, ILogger<ScreenController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly ILogger<ScreenController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet(Name = "GetScreensList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<ScreenVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<ScreenVM>>> GetScreensList([FromQuery] bool WithMeta = false)
        {
            var screens = await _mediator.Send(new GetScreensListQuery { WithMeta = WithMeta });
            
            // Debug: Log the incoming request headers
            // _logger.LogInformation("SCREEN Incoming request headers:");
            // foreach (var header in Request.Headers)
            // {
            //     _logger.LogInformation($"{header.Key}: {header.Value}");
            // }
            return Ok(screens);

        }


        [HttpGet("{id}", Name = "GetScreenDefault")]
        [HttpGet("by-id/{id}", Name = "GetScreenById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(ScreenVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ScreenVM>> GetScreenById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var screen = await _mediator.Send(new GetScreenByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(screen);

        }

        [HttpGet("by-name/{name}", Name = "GetScreenByName")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(ScreenVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ScreenVM>> GetScreenByName(string name, [FromQuery] bool WithMeta = false)
        {
            var screen = await _mediator.Send(new GetScreenByNameQuery { Name = name, WithMeta = WithMeta });
            return Ok(screen);
        }


        [HttpPost(Name = "AddScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddScreen(NewScreenCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Screen added successfully",
            });
        }



        [HttpPut("{id}", Name = "UpdateScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateScreen(Guid id, UpdateScreenCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen updated successfully",
            });
        }


        [HttpPut("{id}/update-associated-targets", Name = "UpdateAssociatedTargets")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAssociatedTargets(Guid id, UpdateScreenAssociatedTargetsCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen associated targets updated successfully",
            });

        }

        [HttpPut("{id}/rename", Name = "RenameScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RenameScreen(Guid id, RenameScreenCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen renamed successfully",
            });
        }

        [HttpDelete("{id}", Name = "DeleteScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteScreen(Guid id)
        {
            await _mediator.Send(new DeleteScreenCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen deleted successfully",
            });
        }


        [HttpPost("import-one", Name = "ImportOne")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> ImportOne(ImportOneCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = command.Id == Guid.Empty ? id : command.Id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Screen imported successfully",
            });

        }
    }
}

using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Target.Application.Features.Command.DeleteTarget;
using Target.Application.Features.Command.NewTarget;
using Target.Application.Features.Command.UpdateTarget;
using Target.Application.Features.Command.UpdateTargetAssociatedGenes;
using Target.Application.Features.Commands.RenameTarget;
using Target.Application.Features.Queries.GetTarget;
using Target.Application.Features.Queries.GetTarget.ById;
using Target.Application.Features.Queries.GetTargetsList;

namespace Target.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class TargetController(IMediator mediator, ILogger<TargetController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly ILogger<TargetController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private void LogRequestHeaders()
        {
            foreach (var header in HttpContext.Request.Headers)
            {
                _logger.LogInformation($"{header.Key}: {header.Value}");
            }
        }

        [HttpGet(Name = "GetTargets")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(TargetsListVM), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<TargetsListVM>> GetTargets([FromQuery] bool WithMeta = false)
        {
            //DEBUG:
            //LogRequestHeaders();
            var targets = await _mediator.Send(new GetTargetsListQuery { WithMeta = WithMeta });
            return Ok(targets);

        }


        [HttpGet("{id}", Name = "GetTargetDefault")]
        [HttpGet("by-id/{id}", Name = "GetTargetById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(TargetVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TargetVM>> GetTargetById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var target = await _mediator.Send(new GetTargetByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(target);
        }



        [HttpPost(Name = "AddTarget")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddTarget(NewTargetCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Target added successfully",
            });
        }



        [HttpPut("{id}", Name = "UpdateTarget")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateTarget(Guid id, UpdateTargetCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Target updated successfully",
            });

        }


        [HttpPut("{id}/update-associated-genes", Name = "UpdateTargetAssociatedGenes")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateTargetAssociatedGenes(Guid id, UpdateTargetAssociatedGenesCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Target associated genes updated successfully",
            });

        }

        [HttpDelete("{id}", Name = "DeleteTarget")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTarget(Guid id)
        {
            await _mediator.Send(new DeleteTargetCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Target deleted successfully",
            });
        }

        // Rename Target
        [HttpPut("{id}/rename", Name = "RenameTarget")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RenameTarget(Guid id, RenameTargetCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Target renamed successfully",
            });

        }
    }
}
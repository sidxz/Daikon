
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Batch.HitHandleUnattachedMolecules;
using Screen.Application.Features.Commands.DeleteHit;
using Screen.Application.Features.Commands.DeleteHitBatch;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Commands.NewHitBatch;
using Screen.Application.Features.Commands.UpdateHit;
using Screen.Application.Features.Commands.UpdateHitBatch;
using Screen.Application.Features.Queries.GetHit.ById;
using Screen.Application.Features.Views.GetHitProperties;
using Screen.Domain.Entities;

namespace Screen.API.Controllers.V2
{
    public partial class HitCollectionController : ControllerBase
    {
        [HttpPost("{id}/hit", Name = "AddHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> AddHit(Guid id, NewHitCommand command)
        {
            var hitId = Guid.NewGuid();
            command.Id = id;
            command.HitId = hitId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = hitId,
                Message = "Hit added successfully",
            });
        }

        [HttpPost("{id}/hit/batch", Name = "AddHitBatch")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddHitBatch(Guid Id, [FromBody] List<NewHitCommand> commands)
        {
            // set the Id for each command
            foreach (var command in commands)
            {
                command.Id = Id;
            }
            var resp = await _mediator.Send(new RegisterHitBatchCommand
            {
                Commands = commands
            });

            return StatusCode(StatusCodes.Status201Created, resp);
        }

        [HttpPut("{id}/hit/{hitId}", Name = "UpdateHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> UpdateHit(Guid id, Guid hitId, UpdateHitCommand command)
        {
            command.Id = id;
            command.HitId = hitId;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit updated successfully",
            });


        }

        [HttpPut("{id}/hit/batch", Name = "UpdateHitBatch")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateHitBatch(Guid id, [FromBody] List<UpdateHitCommand> commands)
        {
            // set the Id for each command
            foreach (var command in commands)
            {
                command.Id = id;
            }
            var resp = await _mediator.Send(new UpdateHitBatchCommand
            {
                Commands = commands
            });

            return StatusCode(StatusCodes.Status200OK, resp);
        }

        [HttpDelete("{id}/hit/{hitId}", Name = "DeleteHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteHit(Guid id, Guid hitId)
        {
            await _mediator.Send(new DeleteHitCommand { Id = id, HitId = hitId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit deleted successfully",
            });
        }

        [HttpDelete("{id}/hit/batch", Name = "DeleteHitBatch")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteHitBatch(Guid id, [FromBody] List<DeleteHitCommand> commands)
        {
            // set the Id for each command
            foreach (var command in commands)
            {
                command.Id = id;
            }
            var resp = await _mediator.Send(new DeleteHitBatchCommand
            {
                Commands = commands
            });

            return StatusCode(StatusCodes.Status200OK, resp);
        }

        [HttpGet("hit/{hitId}", Name = "GetHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Hit>> GetHit(Guid hitId)
        {
            var hit = await _mediator.Send(new GetHitByIdCommand { Id = hitId });
            return StatusCode(StatusCodes.Status200OK, hit);
        }

        [HttpGet("view/hit/{hitId}", Name = "GetHitView")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<HitPropertiesVM>> GetHitView(Guid hitId)
        {
            var hitProperties = await _mediator.Send(new GetHitPropertiesQuery { Id = hitId });
            return StatusCode(StatusCodes.Status200OK, hitProperties);

        }

        [HttpPut("HitMoleculeAttachBatch", Name = "HandleUnattachedMolecules")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> HandleUnattachedMolecules(CancellationToken cancellationToken)
        {
            var command = new HitHandleUnattachedMoleculesCommand();
            await _hitBackgroundService.QueueHandleUnattachedHitsJobAsync(command, cancellationToken);
            return Ok("Hit job queued successfully.");
        }
    }
}
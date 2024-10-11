using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Target.Application.Features.Commands.AddOrUpdateToxicology;
using Target.Application.Features.Commands.AddToxicology;
using Target.Application.Features.Commands.DeleteToxicology;
using Target.Application.Features.Commands.UpdateToxicology;
using Target.Application.Features.Queries.GetToxicology.ByTargetId;
using Target.Application.Features.Queries.GetToxicologyList;

namespace Target.API.Controllers.V2
{
    public partial class TargetController : ControllerBase
    {
        [HttpGet("{id}/toxicology", Name = "GetToxicologyByTarget")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetToxicologyByTarget(Guid id, [FromQuery] bool WithMeta = false)
        {
            var toxicologies = await _mediator.Send(new GetToxicologyByTargetQuery { TargetId = id, WithMeta = WithMeta });

            return Ok(toxicologies);

        }


        [HttpGet("toxicology", Name = "GetToxicologyList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetToxicologyList([FromQuery] bool WithMeta = false)
        {
            var toxicologies = await _mediator.Send(new GetToxicologyListQuery { WithMeta = WithMeta });
            return Ok(toxicologies);
        }

        [HttpPost("{id}/toxicology/add-update", Name = "AddOrUpdateToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddOrUpdateToxicology(Guid id, AddOrUpdateToxicologyCommand command)
        {
            command.Id = id;
            command.TargetId = id;

            await _mediator.Send(command);

            return Ok(new BaseResponse
            {
                Message = "Toxicology added or updated successfully",
            });

        }

        [HttpPost("toxicology/batch-add-update-toxicity", Name = "BatchAddOrUpdateToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> BatchAddOrUpdateToxicology(IEnumerable<AddOrUpdateToxicologyCommand> commands)
        {
            var resp = new BatchResponse
            {
                Success = [],
                Failed = []
            };

            foreach (var command in commands)
            {
                try
                {
                    command.Id = command.TargetId;
                    await _mediator.Send(command);
                    resp.Success.Add(command.TargetId);
                }
                catch (Exception)
                {
                    resp.Failed.Add(command.TargetId);
                }
            }

            return Ok(resp);
        }

        [HttpPost("{id}/toxicology", Name = "AddToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddToxicology(Guid id, AddToxicologyCommand command)
        {
            var toxicologyId = Guid.NewGuid();
            command.ToxicologyId = toxicologyId;
            command.Id = id;
            command.TargetId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = toxicologyId,
                Message = "Toxicology added successfully",
            });
        }


        [HttpPut("{id}/toxicology/{toxicologyId}", Name = "UpdateToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateToxicology(Guid id, Guid toxicologyId, UpdateToxicologyCommand command)
        {
            command.Id = id;
            command.ToxicologyId = toxicologyId;
            command.TargetId = id;

            await _mediator.Send(command);

            return Ok(new BaseResponse
            {
                Message = "Toxicology updated successfully",
            });

        }

        [HttpDelete("{id}/toxicology/{toxicologyId}", Name = "DeleteToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteToxicology(Guid id, Guid toxicologyId)
        {
            var command = new DeleteToxicologyCommand
            {
                Id = id,
                ToxicologyId = toxicologyId,
                TargetId = id
            };

            await _mediator.Send(command);

            return Ok(new BaseResponse
            {
                Message = "Toxicology deleted successfully",
            });

        }
    }
}
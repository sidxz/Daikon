
using System.Net;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HitAssessment.Application.Features.Commands.DeleteHitAssessment;
using HitAssessment.Application.Features.Commands.NewHitAssessment;
using HitAssessment.Application.Features.Commands.UpdateHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessment.ById;
using HitAssessment.Application.Features.Queries.GetHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessmentList;
using HitAssessment.Application.Features.Queries.GetHitAssessment.GetHitAssessmentList;
using HitAssessment.Application.Features.Batch.ImportOne;
using HitAssessment.Application.Features.Commands.RenameHitAssessment;

namespace HitAssessment.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class HitAssessmentController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet(Name = "GetHitAssessmentList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<HitAssessmentListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<HitAssessmentListVM>>> GetHitAssessmentList([FromQuery] bool WithMeta = false)
        {
            var has = await _mediator.Send(new GetHitAssessmentListQuery { WithMeta = WithMeta });
            return Ok(has);
        }


        [HttpGet("{id}", Name = "GetHitAssessmentDefault")]
        [HttpGet("by-id/{id}", Name = "GetHitAssessmentById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(HitAssessmentVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HitAssessmentVM>> GetHitAssessmentById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var ha = await _mediator.Send(new GetHitAssessmentByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(ha);
        }

        [HttpPost(Name = "AddHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddHitAssessment([FromBody] NewHitAssessmentCommand command)
        {

            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Hit Assessment added successfully",
            });
        }


        [HttpPut("{id}", Name = "UpdateHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateHitAssessment(Guid id, UpdateHitAssessmentCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit Assessment updated successfully",
            });
        }



        [HttpDelete("{id}", Name = "DeleteHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHitAssessment(Guid id)
        {
            await _mediator.Send(new DeleteHitAssessmentCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit Assessment deleted successfully",
            });

        }

        [HttpPut("{id}/rename", Name = "RenameHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> RenameHitAssessment(Guid id, [FromBody] RenameHitAssessmentCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit Assessment renamed successfully",
            });

        }


        [HttpPost("import-one", Name = "import-one")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> ImportHitAssessment([FromBody] ImportOneCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = command.Id == Guid.Empty ? id : command.Id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Hit Assessment imported successfully",
            });

        }
    }
}
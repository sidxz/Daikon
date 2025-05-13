
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.BackgroundServices;
using Screen.Application.Features.Commands.ClusterHitCollection;
using Screen.Application.Features.Commands.DeleteHitCollection;
using Screen.Application.Features.Commands.NewHitCollection;
using Screen.Application.Features.Commands.RenameHitCollection;
using Screen.Application.Features.Commands.UpdateHitCollection;
using Screen.Application.Features.Commands.UpdateHitCollectionAssociatedScreen;
using Screen.Application.Features.Queries.GetHitCollection.ById;
using Screen.Application.Features.Queries.GetHitCollectionsOfScreen;

namespace Screen.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class HitCollectionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HitCollectionController> _logger;
        private readonly HitBackgroundService _hitBackgroundService;


        public HitCollectionController(IMediator mediator, ILogger<HitCollectionController> logger, HitBackgroundService hitBackgroundService)
        {
            _mediator = mediator;
            _logger = logger;
            _hitBackgroundService = hitBackgroundService;
        }

        [HttpGet("{id}", Name = "GetHitCollectionById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetHitCollectionById(Guid id)
        {
            var hitCollection = await _mediator.Send(new GetHitCollectionByIdQuery { Id = id });
            return Ok(hitCollection);
        }

        [HttpGet("by-screen/{screenId}", Name = "GetHitCollectionByScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetHitCollectionByScreen(Guid screenId, [FromQuery] bool withMeta = false)
        {

            var hitCollections = await _mediator.Send(new GetHitCollectionsOfScreenQuery { ScreenId = screenId, WithMeta = withMeta });
            return Ok(hitCollections);
        }

        [HttpPost(Name = "AddHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<IActionResult> AddHitCollection(NewHitCollectionCommand command)
        {
            command.Id = Guid.NewGuid();
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = command.Id,
                Message = "Hit collection created successfully",

            });

        }

        [HttpPut("{id}", Name = "UpdateHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> UpdateHitCollection(Guid id, UpdateHitCollectionCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit collection updated successfully",
            });

        }

        [HttpPut("{id}/cluster", Name = "ClusterHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ClusterHitCollection(Guid id)
        {
            var command = new ClusterHitCollectionCommand
            {
                Id = id,
            };
            var resp = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status200OK, resp);

        }

        [HttpDelete("{id}", Name = "DeleteHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHitCollection(Guid id)
        {
            await _mediator.Send(new DeleteHitCollectionCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit collection deleted successfully",
            });

        }

        [HttpPut("{id}/rename", Name = "RenameHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RenameHitCollection(Guid id, RenameHitCollectionCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit Collection renamed successfully",
            });

        }


        [HttpPut("{id}/update-associated-screen", Name = "UpdateAssociatedScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateScreenAssociation(Guid id, UpdateHitCollectionAssociatedScreenCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Hit collection associated screen updated successfully",
            });
        }
    }
}
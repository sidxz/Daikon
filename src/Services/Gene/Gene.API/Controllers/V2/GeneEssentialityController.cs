
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.NewEssentiality;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class GeneEssentialityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GeneEssentialityController> _logger;

        public GeneEssentialityController(IMediator mediator, ILogger<GeneEssentialityController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpPost(Name = "AddEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddEssentiality(NewEssentialityCommand command)
        {
            var essentialityId = Guid.NewGuid();
            try
            {
                command.EssentialityId = essentialityId;
                command.Id = command.GeneId;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = essentialityId,
                    Message = "Essentiality added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddEssentiality: Requested Resource Already Exists {Name}", ex.Message);
                return Conflict(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the essentiality";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = essentialityId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }


    }
}
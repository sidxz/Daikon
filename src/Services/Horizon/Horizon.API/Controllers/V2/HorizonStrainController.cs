
using System.Net;
using Horizon.API.DTOs;
using Horizon.Application.Features.Command.Gene.AddGeneToGraph;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class HorizonStrainController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HorizonStrainController> _logger;

        public HorizonStrainController(IMediator mediator, ILogger<HorizonStrainController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // This is intended for testing only
        // Add Genes to the Graph should only be done via Consumer -> Event Handler -> Graph Repository
        [HttpPost(Name = "AddStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddStrain(AddGeneToGraphCommand command)
        {
            throw new NotImplementedException();
            // var id = Guid.NewGuid();
            // try
            // {
            //     command.Id = id;
            //     await _mediator.Send(command);

            //     return StatusCode(StatusCodes.Status201Created, new AddGeneToGraphResponse
            //     {
            //         Id = id,
            //         Message = "Gene added successfully",
            //     });
            // }
            // catch (InvalidOperationException ex)
            // {
            //     _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
            //     return BadRequest(new BaseResponse
            //     {
            //         Message = ex.Message
            //     });
            // }

            // catch (Exception ex)
            // {
            //     const string SAFE_ERROR_MESSAGE = "An error occurred while adding the gene";
            //     _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

            //     return StatusCode(StatusCodes.Status500InternalServerError, new AddGeneToGraphResponse
            //     {
            //         Id = id,
            //         Message = SAFE_ERROR_MESSAGE
            //     });
            // }
        }
    }
}
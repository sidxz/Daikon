using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MLogix.Application.Features.Commands.RegisterMolecule;

namespace MLogix.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class MoleculeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MoleculeController> _logger;

        public MoleculeController(IMediator mediator, ILogger<MoleculeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost(Name = "RegisterMolecule")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterMolecule([FromBody] RegisterMoleculeCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                RegisterMoleculeResponseDTO registerMoleculeResponseDTO = await _mediator.Send(command);
                return StatusCode(StatusCodes.Status201Created, registerMoleculeResponseDTO);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("RegisterMolecule: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("RegisterMolecule: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the screen";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
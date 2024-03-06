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
using MLogix.Application.Features.Queries.GetMolecule.ById;
using MLogix.Application.Features.Queries.GetMolecule.BySMILES;
using MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId;
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

        [HttpGet("{id}", Name = "GetMoleculeById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculeById(Guid id, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var query = new GetMoleculeByIdQuery { Id = id, WithMeta = WithMeta };
                var molecule = await _mediator.Send(query);
                return Ok(molecule);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetMoleculeById: Resource Not Found {Id}", id);
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching the molecule";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("by-smiles/{smiles}", Name = "GetMoleculeBySMILES")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculeBySMILES(string smiles)
        {
            try
            {
                var query = new GetMoleculeBySMILESQuery { SMILES = smiles };
                var molecule = await _mediator.Send(query);
                return Ok(molecule);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetMoleculeBySMILES: Resource Not Found {SMILES}", smiles);
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching the molecule";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpGet("by-registration/{regId}", Name = "GetMoleculeByRegistrationId")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculeByRegistrationId(Guid regId)
        {
            try
            {
                var query = new GetMoleculeByRegIdQuery{ RegistrationId = regId};
                var molecule = await _mediator.Send(query);
                return Ok(molecule);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetMoleculeByRegistrationId: Resource Not Found {Id}", regId);
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching the molecule";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
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
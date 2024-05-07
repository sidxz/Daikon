using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Horizon.Application.Features.Queries.FindRelatedTarget;
using Horizon.Application.Features.Queries.GenerateHorizon;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Horizon.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class Horizon : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<Horizon> _logger;

        public Horizon(IMediator mediator, ILogger<Horizon> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> Get(Guid id, [FromQuery] bool withMeta = false)
        {
            try
            {
                var horizon = await _mediator.Send(new GenerateHorizonQuery { Id = id, WithMeta = withMeta });

                return Ok(horizon);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("Horizon: ArgumentNullException {id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("Horizon: Requested Resource Not Found {id}", id);
                return NotFound(new BaseResponse
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while generating the Horizon";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("find-target/{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> FindRelatedTarget(Guid id)
        {
            try
            {
                var relatedTarget = await _mediator.Send(new FindRelatedTargetQuery { Id = id });

                return Ok(relatedTarget);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("FindRelatedTarget: ArgumentNullException {id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("FindRelatedTarget: Requested Resource Not Found {id}", id);
                return NotFound(new BaseResponse
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while finding the related target";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
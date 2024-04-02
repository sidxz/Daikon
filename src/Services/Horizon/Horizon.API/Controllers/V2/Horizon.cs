using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Horizon.Application.Features.Queries.GenerateHorizon;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var query = new GenerateHorizonQuery();
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the horizon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while generating the horizon" });
            }
        }
    }
}
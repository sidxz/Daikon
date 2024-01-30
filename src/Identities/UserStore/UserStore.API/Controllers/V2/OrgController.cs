using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserStore.Application.Features.Commands.Orgs.AddOrg;

namespace UserStore.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class OrgController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrgController> _logger;

        public OrgController(IMediator mediator, ILogger<OrgController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Add an Org
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddOrg([FromBody] AddOrgCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Org added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddOrg ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddOrg: Requested Resource Already Exists {Name}", ex.Message);
                return Conflict(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddOrg: An error occurred while adding org.");
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "An error occurred while adding org."
                });
            }
        }
    }
}
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
using UserStore.Application.Features.Commands.Orgs.DeleteOrg;
using UserStore.Application.Features.Queries.Orgs.GetOrg.ById;
using UserStore.Application.Features.Queries.Orgs.ListOrgs;
using UserStore.Domain.Entities;

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

        // List Orgs
        [HttpGet]
        [ProducesResponseType(typeof(List<AppOrg>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListOrgs()
        {
            try
            {
                var response = await _mediator.Send(new ListOrgsQuery());
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid request.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving orgs";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Get Org By Id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppOrg), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrgById(Guid id)
        {
            try
            {
                var response = await _mediator.Send(new GetOrgByIdQuery { Id = id });
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid request.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving org";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
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

        // Delete an Org
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteOrg(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteOrgCommand { Id = id });

                return Ok(new BaseResponse
                {
                    Message = "Org deleted successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("DeleteOrg ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "DeleteOrg: Invalid request.");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteOrg: An error occurred while deleting org.");
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "An error occurred while deleting org."
                });
            }
        }
    }
}
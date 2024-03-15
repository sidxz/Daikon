
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questionnaire.Application.Features.Commands.CreateQuestionnaire;
using Questionnaire.Application.Features.Commands.DeleteQuestionnaire;
using Questionnaire.Application.Features.Commands.UpdateQuestionnaire;
using Questionnaire.Application.Features.Queries.ListQuestionnaires;


namespace Questionnaire.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class QuestionnaireController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<QuestionnaireController> _logger;

        public QuestionnaireController(IMediator mediator, ILogger<QuestionnaireController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // List all questionnaires
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Domain.Entities.Questionnaire>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Questionnaire>>> GetQuestionnaires()
        {
            _logger.LogInformation("GetQuestionnaires - Retrieving all questionnaires");
            try {
                var questionnaires = await _mediator.Send(new ListQuestionnaireQuery());
                return Ok(questionnaires);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetQuestionnaires: Requested Resource Not Found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the screens";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Get a questionnaire by name
        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> GetQuestionnaire(string name)
        {
            _logger.LogInformation("GetQuestionnaire - Retrieving questionnaire by name: {QuestionnaireName}", name);
            try
            {
                var questionnaire = await _mediator.Send(new Application.Features.Queries.GetQuestionnaire.ByName.GetQuestionnaireQuery { Name = name });
                return Ok(questionnaire);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetQuestionnaire: Requested Resource Not Found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the questionnaire";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpGet("by-id/{id}")]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> GetQuestionnaire(Guid id)
        {
            _logger.LogInformation("GetQuestionnaire - Retrieving questionnaire by name: {QuestionnaireId}", id);
            try
            {
                var questionnaire = await _mediator.Send(new Application.Features.Queries.GetQuestionnaire.ById.GetQuestionnaireQuery { Id = id });
                return Ok(questionnaire);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetQuestionnaire: Requested Resource Not Found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the questionnaire";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        // Create a new questionnaire
        [HttpPost]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> CreateQuestionnaire([FromBody] CreateCommand command)
        {
            _logger.LogInformation("CreateQuestionnaire - Creating new questionnaire");
            try
            {
                var questionnaire = await _mediator.Send(command);
                return Ok(questionnaire);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("CreateQuestionnaire: Requested Resource Not Found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while creating the questionnaire";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Update a questionnaire
        [HttpPut]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> UpdateQuestionnaire([FromBody] UpdateCommand command)
        {
            _logger.LogInformation("UpdateQuestionnaire - Updating questionnaire");
            try
            {
                var questionnaire = await _mediator.Send(command);
                return Ok(questionnaire);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateQuestionnaire: Requested Resource Not Found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the questionnaire";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Delete a questionnaire
        [HttpDelete("{name}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteQuestionnaire(string name)
        {
            _logger.LogInformation("DeleteQuestionnaire - Deleting questionnaire by name: {QuestionnaireName}", name);
            try
            {
                await _mediator.Send(new DeleteCommand { Name = name });
                return Ok();
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteQuestionnaire: Requested Resource Not Found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the questionnaire";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }        
    }
}
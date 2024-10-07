
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
    public class QuestionnaireController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        // List all questionnaires
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Domain.Entities.Questionnaire>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Questionnaire>>> GetQuestionnaires()
        {
            var questionnaires = await _mediator.Send(new ListQuestionnaireQuery());
            return Ok(questionnaires);
        }

        // Get a questionnaire by name
        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> GetQuestionnaire(string name)
        {
            var questionnaire = await _mediator.Send(new Application.Features.Queries.GetQuestionnaire.ByName.GetQuestionnaireQuery { Name = name });
            return Ok(questionnaire);
        }


        [HttpGet("by-id/{id}")]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> GetQuestionnaire(Guid id)
        {
            var questionnaire = await _mediator.Send(new Application.Features.Queries.GetQuestionnaire.ById.GetQuestionnaireQuery { Id = id });
            return Ok(questionnaire);
        }


        // Create a new questionnaire
        [HttpPost]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> CreateQuestionnaire([FromBody] CreateCommand command)
        {
            var questionnaire = await _mediator.Send(command);
            return Ok(questionnaire);
        }

        // Update a questionnaire
        [HttpPut]
        [ProducesResponseType(typeof(Domain.Entities.Questionnaire), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Domain.Entities.Questionnaire>> UpdateQuestionnaire([FromBody] UpdateCommand command)
        {
            var questionnaire = await _mediator.Send(command);
            return Ok(questionnaire);

        }

        // Delete a questionnaire
        [HttpDelete("{name}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteQuestionnaire(string name)
        {
            await _mediator.Send(new DeleteCommand { Name = name });
            return Ok();
        }
    }
}
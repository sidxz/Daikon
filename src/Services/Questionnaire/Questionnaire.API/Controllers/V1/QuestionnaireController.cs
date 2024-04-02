
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Questionnaire.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]

    public class QuestionnaireController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<QuestionnaireController> _logger;

        public QuestionnaireController(IMediator mediator, ILogger<QuestionnaireController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        

    }
}
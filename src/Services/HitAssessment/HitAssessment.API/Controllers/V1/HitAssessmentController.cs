
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HitAssessment.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]

    public class HitAssessmentController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<HitAssessmentController> _logger;

        public HitAssessmentController(IMediator mediator, ILogger<HitAssessmentController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        

    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Target.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]

    public class TargetController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<TargetController> _logger;

        public TargetController(IMediator mediator, ILogger<TargetController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        

    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Screen.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]

    public class ScreenController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<ScreenController> _logger;

        public ScreenController(IMediator mediator, ILogger<ScreenController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        

    }
}
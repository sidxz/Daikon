
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Comment.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]

    public class CommentController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        

    }
}
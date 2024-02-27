
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MLogix.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    public class MoleculeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MoleculeController> _logger;

        public MoleculeController(IMediator mediator, ILogger<MoleculeController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gene.API.DTOs;
using Gene.Application.Features.Command.NewGene;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]

    public class GeneController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<GeneController> _logger;

        public GeneController(IMediator mediator, ILogger<GeneController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        

    }
}
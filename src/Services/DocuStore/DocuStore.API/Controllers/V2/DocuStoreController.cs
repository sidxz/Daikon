using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Responses;
using Daikon.Shared.VM.DocuStore;
using DocuStore.Application.Features.Commands.AddParsedDoc;
using DocuStore.Application.Features.Queries.GetParsedDoc.ById;
using DocuStore.Application.Features.Queries.GetParsedDoc.ByTags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocuStore.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/parsed-docs")]
    [ApiVersion("2.0")]
    public partial class DocuStoreController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        // [HttpGet(Name = "GetDocList")]
        // [MapToApiVersion("2.0")]
        // [ProducesResponseType(typeof(IEnumerable<ParsedDocVM>), (int)HttpStatusCode.OK)]
        // public async Task<ActionResult<IEnumerable<ParsedDocVM>> GetDocList([FromQuery] bool WithMeta = false)
        // {
        //     var parsedDocs = await _mediator.Send(new GetParsedDocListQuery { WithMeta = WithMeta });
        //     return Ok(parsedDocs);

        // }

        [HttpGet("{id}", Name = "GetDocDefault")]
        [HttpGet("by-id/{id}", Name = "GetDocById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(ParsedDocVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParsedDocVM>> GetDocById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var parsedDoc = await _mediator.Send(new GetParsedDocByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(parsedDoc);
        }

        [HttpGet("by-tags", Name = "GetDocsByTags")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<ParsedDocVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ParsedDocVM>>> GetDocsByTags([FromQuery] HashSet<string> tags, [FromQuery] bool WithMeta = false)
        {
            var parsedDocs = await _mediator.Send(new GetParsedDocByTagsQuery { Tags = tags, WithMeta = WithMeta });
            return Ok(parsedDocs);
        }

        [HttpPost(Name = "AddDoc")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddDoc(AddParsedDocCommand command)
        {
            command.Id = command.Id == Guid.Empty ? Guid.NewGuid() : command.Id;
            var res = await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, res);
        }

        [HttpPut("{id}", Name = "UpdateDoc")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateDoc(Guid id, AddParsedDocCommand command)
        {
            command.Id = id;
            var res = await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, res);
        }


    }
}
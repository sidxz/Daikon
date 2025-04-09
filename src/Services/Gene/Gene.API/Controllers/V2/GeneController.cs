
using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteGene;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.UpdateGene;
using Gene.Application.Features.Queries.GetGene;
using Gene.Application.Features.Queries.GetGene.ByAccession;
using Gene.Application.Features.Queries.GetGene.ById;
using Gene.Application.Features.Queries.GetGenesList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]

    public partial class GeneController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet(Name = "GetGenesList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<GenesListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<GenesListVM>>> GetGenesList()
        {

            var genesList = await _mediator.Send(new GetGenesListQuery());
            return Ok(genesList);
        }

        [HttpGet("{id}", Name = "GetGeneDefault")]
        [HttpGet("by-id/{id}", Name = "GetGeneById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(GeneVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GeneVM>> GetGeneById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var gene = await _mediator.Send(new GetGeneByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(gene);
        }


        [HttpGet("by-accession/{accessionNo}", Name = "GetGeneByAccession")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(GeneVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GeneVM>> GetGeneByAccession(string accessionNo, [FromQuery] bool WithMeta = false)
        {
            var gene = await _mediator.Send(new GetGeneByAccessionQuery { AccessionNumber = accessionNo, WithMeta = WithMeta });
            return Ok(gene);
        }


        [HttpPost(Name = "AddGene")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddGene(NewGeneCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Gene added successfully",
            });
        }

        [HttpPut("{id}", Name = "UpdateGene")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateGene(Guid id, UpdateGeneCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Gene updated successfully",
            });

        }

        [HttpDelete("{id}", Name = "DeleteGene")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteGene(Guid id)
        {
            await _mediator.Send(new DeleteGeneCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Gene deleted successfully",
            });

        }
    }
}

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Queries.GetMolecule.ById;
using MLogix.Application.Features.Queries.GetMolecule.BySMILES;
using MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId;
using MLogix.Application.Features.Queries.ListMolecules;
using MLogix.Application.Features.Queries.FindSimilarMolecules;
using MLogix.Application.Features.Commands.UpdateMolecule;
namespace MLogix.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class MoleculeController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet(Name = "GetAllMolecules")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllMolecules()
        {
            var query = new ListMoleculesCommand { };
            var molecules = await _mediator.Send(query);
            return Ok(molecules);

        }

        [HttpGet("{id}", Name = "GetMoleculeById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculeById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var query = new GetMoleculeByIdQuery { Id = id, WithMeta = WithMeta };
            var molecule = await _mediator.Send(query);
            return Ok(molecule);

        }

        [HttpGet("by-smiles/{smiles}", Name = "GetMoleculeBySMILES")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculeBySMILES(string smiles)
        {
            var query = new GetMoleculeBySMILESQuery { SMILES = smiles };
            var molecule = await _mediator.Send(query);
            return Ok(molecule);

        }


        [HttpGet("by-registration/{regId}", Name = "GetMoleculeByRegistrationId")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculeByRegistrationId(Guid regId)
        {
            var query = new GetMoleculeByRegIdQuery { RegistrationId = regId };
            var molecule = await _mediator.Send(query);
            return Ok(molecule);

        }

        [HttpGet("similar/{smiles}", Name = "FindSimilarMolecules")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> FindSimilarMolecules(string smiles, [FromQuery] double similarityThreshold = 1, [FromQuery] int maxResults = 10, [FromQuery] bool WithMeta = false)
        {
            var query = new FindSimilarMoleculesQuery { SMILES = smiles, SimilarityThreshold = similarityThreshold, MaxResults = maxResults, WithMeta = WithMeta };
            var molecules = await _mediator.Send(query);
            return Ok(molecules);

        }

        [HttpPost(Name = "RegisterMolecule")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterMolecule([FromBody] RegisterMoleculeCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            RegisterMoleculeResponseDTO registerMoleculeResponseDTO = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, registerMoleculeResponseDTO);

        }

        [HttpPut("{id}", Name = "UpdateMolecule")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateMolecule(Guid id, [FromBody] UpdateMoleculeCommand command)
        {
            command.Id = id;
            var updateMoleculeResponseDTO = await _mediator.Send(command);
            return Ok(updateMoleculeResponseDTO);

        }
    }
}
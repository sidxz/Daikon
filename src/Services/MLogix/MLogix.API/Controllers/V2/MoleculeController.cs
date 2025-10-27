
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Queries.GetMolecule.ById;
using MLogix.Application.Features.Queries.GetMolecule.BySMILES;
using MLogix.Application.Features.Queries.GetMolecule.ByRegistrationId;
using MLogix.Application.Features.Queries.FindSimilarMolecules;
using MLogix.Application.Features.Commands.UpdateMolecule;
using MLogix.Application.Features.Queries.FindSubstructures;
using MLogix.Application.Features.Queries.GetMolecule.ByName;
using MLogix.Application.Features.Queries.GetMolecules.ByIDs;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;
using MLogix.Application.Features.Commands.ReregisterVault;
using MLogix.Application.BackgroundServices;
using MLogix.Application.Features.Commands.GenerateParentBatch;
using MLogix.Application.Features.Commands.DiscloseMolecule;
using MLogix.Application.Features.Previews.DiscloseMoleculePreview;
using MLogix.Application.Features.Calculations.Clustering;
using Daikon.Shared.DTO.MLogix;
using MLogix.Application.Features.Queries.GetRecentDisclosures;
using MLogix.Application.Features.Previews.RegisterMoleculePreview;
namespace MLogix.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class MoleculeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly VaultBackgroundServices _vaultBackgroundServices;

        public MoleculeController(IMediator mediator, VaultBackgroundServices vaultBackgroundServices)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _vaultBackgroundServices = vaultBackgroundServices ?? throw new ArgumentNullException(nameof(vaultBackgroundServices));
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
        [HttpPost("by-ids", Name = "GetMoleculesByIds")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMoleculesByIds([FromBody] GetMoleculeByIDsQuery query)
        {
            var molecules = await _mediator.Send(query);
            return Ok(molecules);
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

        [HttpGet("by-name", Name = "GetMoleculesByName")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> FindMoleculesByName([FromQuery] GetByNameQuery query)
        {
            var molecules = await _mediator.Send(query);
            return Ok(molecules);
        }


        [HttpGet("similar", Name = "FindSimilarMolecules")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> FindSimilarMolecules([FromQuery] FindSimilarMoleculesQuery query)
        {
            var molecules = await _mediator.Send(query);
            return Ok(molecules);
        }

        [HttpGet("substructure", Name = "FindSubstructures")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> FindSubstructures([FromQuery] FindSubstructuresQuery query)
        {
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

        [HttpPost("register-molecule-preview", Name = "RegisterMoleculePreview")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterMoleculePreview([FromBody] RegisterMoleculePreviewQuery query)
        {
            var results = await _mediator.Send(query);
            return Ok(results);
        }


        [HttpPost("batch", Name = "RegisterMoleculeBatch")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterMoleculeBatch([FromBody] List<RegisterMoleculeCommandWithRegId> commands)
        {
            var batchCommand = new RegisterMoleculeBatchCommand { Commands = commands };
            var response = await _mediator.Send(batchCommand);
            return Ok(response);
        }



        [HttpPost("reregister-vault", Name = "ReregisterVault")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public IActionResult ReregisterVault([FromBody] ReRegisterVaultCommand command)
        {
            // Queue the background job
            _ = _vaultBackgroundServices.QueueReregisterVaultJobAsync(command, HttpContext.RequestAborted);
            return Accepted("ReregisterVault job has been queued and is processing in the background.");
        }

        [HttpPost("generate-parent-batch", Name = "GenerateParentBatch")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public IActionResult GenerateParent([FromBody] GenerateParentBatchCommand command)
        {
            // Queue the background job
            _ = _vaultBackgroundServices.QueueGenerateParentBatchJobAsync(command, HttpContext.RequestAborted);
            return Accepted("GenerateParentBatch job has been queued and is processing in the background.");
        }



        [HttpPost("disclose-molecule-preview", Name = "DiscloseMoleculePreview")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DiscloseMoleculePreview([FromBody] DiscloseMoleculePreviewQuery query)
        {
            var results = await _mediator.Send(query);
            return Ok(results);
        }


        [HttpPut("disclose", Name = "DiscloseMolecule")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DiscloseMolecule([FromBody] DiscloseMoleculeCommand command)
        {
            var discloseMoleculeResponseDTO = await _mediator.Send(command);
            return Ok(discloseMoleculeResponseDTO);
        }

        [HttpPut("disclose-batch", Name = "DiscloseMoleculeBatch")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DiscloseMoleculeBatch([FromBody] DiscloseMoleculeBatchCommand command)
        {
            var discloseMoleculeResponseDTO = await _mediator.Send(command);
            return Ok(discloseMoleculeResponseDTO);
        }


        // cluster
        [HttpPost("cluster", Name = "GenerateCluster")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GenerateCluster(double CutOff, [FromBody] List<ClusterDTO> molecules)
        {
            var command = new GenerateClusterCommand

            {
                CutOff = CutOff,
                Molecules = molecules
            };
            {

                var clusterResults = await _mediator.Send(command);
                return Ok(clusterResults);
            }
        }


        [HttpGet("recent-disclosure", Name = "GetRecentDisclosures")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRecentDisclosures([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var query = new GetRecentDisclosuresQuery { StartDate = startDate, EndDate = endDate };
            var molecules = await _mediator.Send(query);
            return Ok(molecules);
        }
    }
}
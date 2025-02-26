
using Horizon.Application.Features.Queries.CompoundRelations;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.API.Controllers.V2
{
    public partial class Horizon
    {
        [HttpGet("find-molecule-relations/{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> FindMoleculeRelations(Guid id)
        {
            var relations = await _mediator.Send(new CompoundRelationsQuery { Id = id });

            return Ok(relations);

        }

        [HttpPost("find-molecule-relations")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> FindMoleculeRelationsMultiple([FromBody] CompoundRelationsMultipleQuery request)
        {
            var relations = await _mediator.Send(request);

            return Ok(relations);
        }
    }
}
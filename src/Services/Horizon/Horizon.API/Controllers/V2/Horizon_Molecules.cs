
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
    }
}
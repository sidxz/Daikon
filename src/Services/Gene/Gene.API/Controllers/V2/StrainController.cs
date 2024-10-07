
using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteStrain;
using Gene.Application.Features.Command.NewStrain;
using Gene.Application.Features.Command.UpdateStrain;
using Gene.Application.Features.Queries.GetStrain;
using Gene.Application.Features.Queries.GetStrain.ById;
using Gene.Application.Features.Queries.GetStrain.ByName;
using Gene.Application.Features.Queries.GetStrainsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class StrainController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet(Name = "GetStrainsList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<StrainsListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<StrainsListVM>>> GetStrainsList()
        {
            var strainsList = await _mediator.Send(new GetStrainsListQuery());
            return Ok(strainsList);
        }


        [HttpGet("{id}", Name = "GetStrainDefault")]
        [HttpGet("by-id/{id}", Name = "GetStrainById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(StrainVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StrainVM>> GetStrainById(Guid id)
        {
            var strain = await _mediator.Send(new GetStrainByIdQuery { Id = id });
            return Ok(strain);
        }


        [HttpGet("by-name/{name}", Name = "GetStrainByName")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(StrainVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StrainVM>> GetStrainByName(string name)
        {
            var strain = await _mediator.Send(new GetStrainByNameQuery { Name = name });
            return Ok(strain);
        }

        [HttpPost(Name = "AddStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddStrain(NewStrainCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Strain added successfully",
            });
        }

        [HttpPut("{id}", Name = "UpdateStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateStrain(Guid id, UpdateStrainCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Strain updated successfully",
            });
        }


        [HttpDelete("{id}", Name = "DeleteStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteStrain(Guid id)
        {
            await _mediator.Send(new DeleteStrainCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Strain deleted successfully",
            });

        }
    }
}
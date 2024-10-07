
using System.Net;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteProteinActivityAssay;
using Gene.Application.Features.Command.NewProteinActivityAssay;
using Gene.Application.Features.Command.UpdateProteinActivityAssay;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {


        [HttpPost("{id}/protein-activity-assay", Name = "AddProteinActivityAssay")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddProteinActivityAssay(Guid id, NewProteinActivityAssayCommand command)
        {
            var proteinActivityAssayId = Guid.NewGuid();
            command.ProteinActivityAssayId = proteinActivityAssayId;
            command.Id = id;
            command.GeneId = id;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = proteinActivityAssayId,
                Message = "Protein activity assay added successfully",
            });
        }


        [HttpPut("{id}/protein-activity-assay/{proteinActivityAssayId}", Name = "UpdateProteinActivityAssay")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProteinActivityAssay(Guid id, Guid proteinActivityAssayId, UpdateProteinActivityAssayCommand command)
        {
            command.Id = id;
            command.ProteinActivityAssayId = proteinActivityAssayId;

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Protein activity assay updated successfully",
            });
        }

        [HttpDelete("{id}/protein-activity-assay/{proteinActivityAssayId}", Name = "DeleteProteinActivityAssay")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProteinActivityAssay(Guid id, Guid proteinActivityAssayId)
        {
            await _mediator.Send(new DeleteProteinActivityAssayCommand { Id = id, GeneId = id, ProteinActivityAssayId = proteinActivityAssayId });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Protein activity assay deleted successfully",
            });
        }
    }
}
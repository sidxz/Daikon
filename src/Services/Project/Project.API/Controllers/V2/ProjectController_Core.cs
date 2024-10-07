
using System.Net;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Features.Commands.DeleteProject;
using Project.Application.Features.Commands.NewProject;
using Project.Application.Features.Commands.UpdateProject;
using Project.Application.Features.Queries.GetProject.ById;
using Project.Application.Features.Queries.GetProject;
using Project.Application.Features.Queries.GetProjectList;
using Project.Application.Features.Commands.UpdateProjectAssociation;
using Project.Application.Features.Batch;
using Project.Application.Features.Commands.RenameProject;


namespace Project.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class ProjectController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet(Name = "GetProjectList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<ProjectListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProjectListVM>>> GetProjectList([FromQuery] bool WithMeta = false)
        {
            var projects = await _mediator.Send(new GetProjectListQuery { WithMeta = WithMeta });
            return Ok(projects);

        }


        [HttpGet("{id}", Name = "GetProjectDefault")]
        [HttpGet("by-id/{id}", Name = "GetProjectById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(ProjectVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectVM>> GetProjectById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(project);

        }


        [HttpPost(Name = "AddProject")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddProject(NewProjectCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Project added successfully",
            });
        }

        [HttpPut("{id}", Name = "UpdateProject")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProject(Guid id, UpdateProjectCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project updated successfully",
            });

        }


        [HttpPut("{id}/update-association", Name = "UpdateProjectAssociation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProjectAssociation(Guid id, UpdateProjectAssociationCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project association updated successfully",
            });
        }



        [HttpDelete("{id}", Name = "DeleteProject")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProject(Guid id)
        {
            await _mediator.Send(new DeleteProjectCommand { Id = id });

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project deleted successfully",
            });

        }

        [HttpPut("{id}/rename", Name = "RenameProject")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> RenameProject(Guid id, RenameProjectCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project renamed successfully",
            });

        }

        [HttpPost("import-one", Name = "import-one")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> ImportOne(ImportOneCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = command.Id == Guid.Empty ? id : command.Id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse
            {
                Message = "Project imported successfully",
            });
        }
    }
}
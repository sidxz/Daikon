
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Comment.Application.Features.Commands.NewComment;
using Comment.Application.Features.Commands.UpdateComment;
using Comment.Application.Features.Commands.DeleteComment;
using Comment.Application.Features.Queries.GetComment;
using Comment.Application.Features.Queries.GetComment.ById;
using Comment.Application.Features.Queries.GetCommentList;
using Comment.Application.Features.Queries.GetComment.ByTags;
using Comment.Application.Features.Queries.GetMostRecent;


namespace Comment.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]

    public partial class CommentController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet(Name = "GetCommentList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CommentListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CommentListVM>>> GetCommentList([FromQuery] bool WithMeta = false)
        {
            var comments = await _mediator.Send(new GetCommentListQuery { WithMeta = WithMeta });
            return Ok(comments);

        }

        [HttpGet("{id}", Name = "GetCommentDefault")]
        [HttpGet("by-id/{id}", Name = "GetCommentById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(CommentVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<CommentVM>> GetCommentById(Guid id, [FromQuery] bool WithMeta = false)
        {
            var comment = await _mediator.Send(new GetCommentByIdQuery { Id = id, WithMeta = WithMeta });
            return Ok(comment);
        }


        // get by tags
        [HttpGet("by-tags", Name = "GetCommentsByTags")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CommentVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CommentVM>>> GetCommentsByTags([FromQuery] HashSet<string> tags, [FromQuery] bool WithMeta = false)

        {
            var comments = await _mediator.Send(new GetCommentsByTagsQuery { Tags = tags, WithMeta = WithMeta });
            return Ok(comments);
        }



        [HttpPost(Name = "AddComment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> AddComment([FromBody] NewCommentCommand command)
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Comment added successfully",
            });

        }

        [HttpPut("{id}", Name = "UpdateComment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> UpdateComment(Guid id, UpdateCommentCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = id,
                Message = "Comment updated successfully",
            });

        }

        [HttpDelete("{id}", Name = "DeleteComment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> DeleteComment(Guid id)
        {
            await _mediator.Send(new DeleteCommentCommand { Id = id });

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Message = "Comment deleted successfully",
            });

        }

        [HttpGet("most-recent", Name = "GetMostRecent")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CommentVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CommentVM>>> GetMostRecent([FromQuery] int Count)
        {
            Count = Count > 0 ? Count : 5;
            var comments = await _mediator.Send(new GetMostRecentQuery { Count = Count });
            return Ok(comments);
        }

    }

}
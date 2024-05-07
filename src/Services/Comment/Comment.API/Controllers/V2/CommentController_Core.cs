
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

    public partial class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetCommentList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CommentListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CommentListVM>>> GetCommentList([FromQuery] bool WithMeta = false)
        {
            try
            {
                var comments = await _mediator.Send(new GetCommentListQuery { WithMeta = WithMeta });
                return Ok(comments);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the comment list";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("{id}", Name = "GetCommentDefault")]
        [HttpGet("by-id/{id}", Name = "GetCommentById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(CommentVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<CommentVM>> GetCommentById(Guid id, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var comment = await _mediator.Send(new GetCommentByIdQuery { Id = id, WithMeta = WithMeta });
                return Ok(comment);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetCommentById: Requested Resource Not Found {Id}", id);
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                 _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the comment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        // get by tags
        [HttpGet("by-tags", Name = "GetCommentsByTags")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CommentVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CommentVM>>> GetCommentsByTags([FromQuery] HashSet<string> tags, [FromQuery] bool WithMeta = false)

        {
            try
            {
                var comments = await _mediator.Send(new GetCommentsByTagsQuery { Tags = tags, WithMeta = WithMeta });
                return Ok(comments);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the comment list";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
        


        [HttpPost(Name = "AddComment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> AddComment([FromBody] NewCommentCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Comment added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddProject: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddProject: Requested Resource Already Exists {Name}", ex.Message);
                return Conflict(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the comment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPut("{id}", Name = "UpdateComment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> UpdateComment(Guid id, UpdateCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Comment added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddProject: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddProject: Requested Resource Already Exists {Name}", ex.Message);
                return Conflict(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the comment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpDelete("{id}", Name = "DeleteComment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> DeleteComment(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteCommentCommand { Id = id });

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Message = "Comment deleted successfully",
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteComment: Requested Resource Not Found {Id}", id);
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the comment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }

        [HttpGet("most-recent", Name = "GetMostRecent")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<CommentVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CommentVM>>> GetMostRecent([FromQuery] int Count)
        {
            try
            {
                Count = Count > 0 ? Count : 5;
                var comments = await _mediator.Send(new GetMostRecentQuery { Count = Count });
                return Ok(comments);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the comment list";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

    }

}
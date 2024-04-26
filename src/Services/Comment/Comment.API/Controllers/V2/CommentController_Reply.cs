using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Comment.Application.Features.Commands.NewCommentReply;
using Comment.Application.Features.Commands.UpdateCommentReply;
using Comment.Application.Features.Commands.DeleteCommentReply;
using Microsoft.AspNetCore.Mvc;

namespace Comment.API.Controllers.V2
{
    public partial class CommentController : ControllerBase
    {
        [HttpPost("{commentId}/reply/", Name = "AddCommentReply")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddCommentReply(Guid commentId, NewCommentReplyCommand command)
        {
            command.Id = commentId;
            var commentReplyId = Guid.NewGuid();
            try
            {
                command.ReplyId = commentReplyId;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = commentReplyId,
                    Message = "Comment Reply added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddCommentReply: ArgumentNullException {Id}", commentReplyId);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddCommentReply: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the Comment Reply";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPut("{commentId}/reply/{id}", Name = "UpdateCommentReply")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateCommentReply(Guid commentId, Guid id, UpdateCommentReplyCommand command)
        {
            command.Id = commentId;
            command.CommentId = commentId;
            command.ReplyId = id;
            try
            {
                await _mediator.Send(command);

                return Ok(new BaseResponse
                {
                    Message = "Comment Reply updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateCommentReply: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateCompoundEvolution: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the Compound Evolution";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpDelete("{commentId}/reply/{id}", Name = "DeleteCommentReply")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> DeleteCommentReply(Guid commentId, Guid id)
        {

            var command = new DeleteCommentReplyCommand
            {
                ReplyId = id,
                Id = commentId,
            };
            

            try
            {
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Message = "Comment Reply deleted successfully",
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteCommentReply: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the Compound Evolution";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

    }

}
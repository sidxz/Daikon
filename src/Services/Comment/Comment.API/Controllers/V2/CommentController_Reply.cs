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
            command.ReplyId = commentReplyId;
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = commentReplyId,
                Message = "Reply added successfully",
            });

        }

        [HttpPut("{commentId}/reply/{id}", Name = "UpdateCommentReply")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateCommentReply(Guid commentId, Guid id, UpdateCommentReplyCommand command)
        {
            command.Id = commentId;
            command.CommentId = commentId;
            command.ReplyId = id;
            await _mediator.Send(command);

            return Ok(new BaseResponse
            {
                Message = "Reply updated successfully",
            });
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

            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Message = "Reply deleted successfully",
            });
        }
    }
}
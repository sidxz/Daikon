using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Comment.Application.Features.Queries.GetComment
{
    public class CommentReplyVM :DocMetadata
    {
        public Guid Id { get; set; }
        public DVariable<string> Body { get; set; }
        public HashSet<string> Tags { get; set; }
        public HashSet<Guid> Mentions { get; set; }
        public HashSet<Guid> Subscribers { get; set; }
    }
}
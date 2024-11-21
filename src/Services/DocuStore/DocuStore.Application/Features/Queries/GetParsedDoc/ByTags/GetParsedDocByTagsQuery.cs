using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Daikon.Shared.VM.DocuStore;
using MediatR;

namespace DocuStore.Application.Features.Queries.GetParsedDoc.ByTags
{
    public class GetParsedDocByTagsQuery : BaseQuery, IRequest<List<ParsedDocVM>>
    {
        public required HashSet<string> Tags { get; set; }
        
    }
}
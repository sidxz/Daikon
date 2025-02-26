using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Daikon.Shared.VM.DocuStore;
using MediatR;

namespace DocuStore.Application.Features.Queries.GetParsedDoc.ByPath
{
    public class GetParsedDocByPathQuery : BaseQuery, IRequest<ParsedDocVM>
    {
        public required string Path { get; set; }
    }
}
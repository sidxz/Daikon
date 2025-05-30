using System;
using System.Collections.Generic;
using CQRS.Core.Query;
using MediatR;

namespace MLogix.Application.Features.Previews.DiscloseMoleculePreview
{
    public class DiscloseMoleculePreviewQuery : BaseQuery, IRequest<List<DiscloseMoleculePreviewDTO>>
    {
        public List<QueryItem> Queries { get; set; } = [];
    }

    public class QueryItem
    {
        public string Name { get; set; }
        public string SMILES { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace MLogix.Application.Features.Previews.RegisterMoleculePreview
{

    public class RegisterMoleculePreviewQuery : BaseQuery, IRequest<List<RegisterMoleculePreviewDTO>>
    {
        public List<QueryItem> Queries { get; set; } = [];
    }

    public class QueryItem
    {
        public string Name { get; set; }
        public string SMILES { get; set; }
    }
}
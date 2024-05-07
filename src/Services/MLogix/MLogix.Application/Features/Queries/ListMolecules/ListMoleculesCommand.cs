using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Query;
using MediatR;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.Contracts.Persistence;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.ListMolecules
{
    public class ListMoleculesCommand : BaseQuery, IRequest<List<MoleculeListVM>>
    {
        
    }
}
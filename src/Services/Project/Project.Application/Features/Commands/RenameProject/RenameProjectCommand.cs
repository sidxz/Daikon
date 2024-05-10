using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Project.Application.Features.Commands.RenameProject
{
    public class RenameProjectCommand : BaseCommand, IRequest<Unit>
    {
        public string Name { get; set; }
    }
}
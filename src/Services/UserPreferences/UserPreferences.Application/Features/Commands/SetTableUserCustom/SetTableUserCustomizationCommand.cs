using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Features.Commands.SetTableUserCustom
{
    public class SetTableUserCustomizationCommand : BaseCommand, IRequest<TableUserCustomization>
    {
        public required string TableType { get; set; }
        public required Guid TableInstanceId { get; set; }
        public List<string> Columns { get; set; } = [];
        public DateTime LastModified { get; set; }
        public int Version { get; set; }
    }
}
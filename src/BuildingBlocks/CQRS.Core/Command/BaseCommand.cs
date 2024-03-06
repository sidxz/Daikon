using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Messages;

/// <summary>
/// Base class for all commands.
/// </summary>
namespace CQRS.Core.Command
{
    public abstract class BaseCommand : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid RequestorUserId { get; set;}
    }
}
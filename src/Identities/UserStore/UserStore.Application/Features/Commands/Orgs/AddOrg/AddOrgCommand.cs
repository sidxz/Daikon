using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Orgs.AddOrg
{
    public class AddOrgCommand : IRequest<AppOrg>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool? IsInternal { get; set; }
    }
}
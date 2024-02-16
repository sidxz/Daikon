using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using UserStore.Application.Features.Queries.Users.GetUser.VMs;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.Users.GetUser.ById
{
    public class GetUserByIdQuery : IRequest<AppUserVM>
    {
        public Guid Id { get; set; }
    }
}
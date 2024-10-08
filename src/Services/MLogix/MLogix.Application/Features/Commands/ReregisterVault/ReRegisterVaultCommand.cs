
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.ReregisterVault
{
    public class ReRegisterVaultCommand : BaseCommand, IRequest<Unit>
    {

    }
}
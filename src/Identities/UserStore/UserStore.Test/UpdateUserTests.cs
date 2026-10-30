using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UserStore.Application.Contracts.Persistence;
using UserStore.Application.Features.Commands.Users.UpdateUser;
using UserStore.Application.Mappings;
using UserStore.Domain.Entities;
using Xunit;

namespace UserStore.Test;

public class UpdateUserTests
{
    [Fact]
    public async Task Changing_the_email_keeps_the_normalised_email_in_step()
    {
        // Sign-in compares the token's email with NormalizedEmail; if only Email changes, the user is locked out
        var id = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var users = new Mock<IAppUserRepository>();
        users.Setup(r => r.GetUserById(id)).ReturnsAsync(new AppUser
        {
            Id = id, Email = "ada@example.org", NormalizedEmail = "ADA@EXAMPLE.ORG", AppOrgId = orgId,
        });
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        var handler = new UpdateUserHandler(mapper, NullLogger<UpdateUserHandler>.Instance, users.Object,
            Mock.Of<IAppRoleRepository>(), Mock.Of<IAppOrgRepository>());

        await handler.Handle(new UpdateUserCommand { Id = id, Email = "Ada.Lovelace@example.org", AppOrgId = orgId }, default);

        users.Verify(r => r.UpdateUser(It.Is<AppUser>(u =>
            u.Email == "Ada.Lovelace@example.org" && u.NormalizedEmail == "ADA.LOVELACE@EXAMPLE.ORG")), Times.Once);
    }
}

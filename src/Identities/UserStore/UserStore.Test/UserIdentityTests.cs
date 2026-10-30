using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Web;
using Moq;
using UserStore.API;
using UserStore.Application.Contracts.Persistence;
using UserStore.Application.Features.Commands.Users.OAuth2Connect;
using UserStore.Application.Features.Commands.Users.ValidateUserAccess;
using UserStore.Domain.Entities;
using Xunit;

namespace UserStore.Test;

// Every service's first request for a user ends up here: this decides who the token's owner is in DAIKON
public class UserIdentityTests
{
    private const string Oid = "entra-oid-1";

    private static AppUser User(string? entraObjectId = Oid, bool locked = false) => new()
    {
        Id = Guid.NewGuid(),
        Email = "ada@example.org",
        NormalizedEmail = "ADA@EXAMPLE.ORG",
        EntraObjectId = entraObjectId!,
        IsUserLocked = locked,
    };

    private static (ValidateUserAccessHandler Handler, Mock<IAppUserRepository> Users, Mock<IMediator> Mediator) Validator()
    {
        var users = new Mock<IAppUserRepository>();
        var mediator = new Mock<IMediator>();
        var handler = new ValidateUserAccessHandler(null!, NullLogger<ValidateUserAccessHandler>.Instance, users.Object, mediator.Object);
        return (handler, users, mediator);
    }

    [Fact]
    public async Task Known_entra_user_is_valid()
    {
        var (handler, users, _) = Validator();
        var user = User();
        users.Setup(r => r.GetUserByEntraObjectId(Oid)).ReturnsAsync(user);

        var result = await handler.Handle(new ValidateUserAccessCommand { EntraObjectId = Oid, Email = "ada@example.org" }, default);

        Assert.True(result.IsValid);
        Assert.Equal(user.Id, result.AppUserId);
    }

    [Fact]
    public async Task Locked_user_is_rejected()
    {
        var (handler, users, _) = Validator();
        users.Setup(r => r.GetUserByEntraObjectId(Oid)).ReturnsAsync(User(locked: true));

        var result = await handler.Handle(new ValidateUserAccessCommand { EntraObjectId = Oid, Email = "ada@example.org" }, default);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Linked_account_with_a_different_token_email_is_rejected()
    {
        var (handler, users, _) = Validator();
        users.Setup(r => r.GetUserByEntraObjectId(Oid)).ReturnsAsync(User());

        var result = await handler.Handle(new ValidateUserAccessCommand { EntraObjectId = Oid, Email = "mallory@example.org" }, default);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task First_sign_in_links_the_token_identity_to_the_account_with_that_email()
    {
        var (handler, users, mediator) = Validator();
        var user = User(entraObjectId: null);
        users.Setup(r => r.GetUserByEmail("ada@example.org")).ReturnsAsync(user);

        var result = await handler.Handle(new ValidateUserAccessCommand { EntraObjectId = Oid, Email = "ada@example.org" }, default);

        Assert.True(result.IsValid);
        mediator.Verify(m => m.Send(It.Is<OAuth2ConnectCommand>(c => c.EntraObjectId == Oid && c.Email == user.Email), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Locked_account_is_not_linked_on_first_sign_in()
    {
        var (handler, users, mediator) = Validator();
        users.Setup(r => r.GetUserByEmail("ada@example.org")).ReturnsAsync(User(entraObjectId: null, locked: true));

        var result = await handler.Handle(new ValidateUserAccessCommand { EntraObjectId = Oid, Email = "ada@example.org" }, default);

        Assert.False(result.IsValid);
        mediator.Verify(m => m.Send(It.IsAny<OAuth2ConnectCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Linking_never_overwrites_an_existing_entra_id()
    {
        var users = new Mock<IAppUserRepository>();
        users.Setup(r => r.GetUserByEmail("ada@example.org")).ReturnsAsync(User(entraObjectId: "already-linked"));
        var handler = new OAuth2ConnectHandler(null!, NullLogger<OAuth2ConnectHandler>.Instance, users.Object);

        await Assert.ThrowsAsync<AccountLinkingRefusedException>(() =>
            handler.Handle(new OAuth2ConnectCommand { Email = "ada@example.org", EntraObjectId = Oid }, default));
        users.Verify(r => r.UpdateUser(It.IsAny<AppUser>()), Times.Never);
    }

    [Fact]
    public async Task Linking_refuses_an_entra_id_that_belongs_to_another_account()
    {
        var users = new Mock<IAppUserRepository>();
        users.Setup(r => r.GetUserByEmail("ada@example.org")).ReturnsAsync(User(entraObjectId: null));
        users.Setup(r => r.GetUserByEntraObjectId(Oid)).ReturnsAsync(User());
        var handler = new OAuth2ConnectHandler(null!, NullLogger<OAuth2ConnectHandler>.Instance, users.Object);

        await Assert.ThrowsAsync<AccountLinkingRefusedException>(() =>
            handler.Handle(new OAuth2ConnectCommand { Email = "ada@example.org", EntraObjectId = Oid }, default));
        users.Verify(r => r.UpdateUser(It.IsAny<AppUser>()), Times.Never);
    }

    [Fact]
    public async Task Refused_linking_means_not_a_valid_user_rather_than_an_error()
    {
        // As the MediatR pipeline delivers it: RequestorIdBehavior wraps the handler's exception
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<ValidateUserAccessCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("An error occurred while processing the request.",
                new AccountLinkingRefusedException("User already has a EntraObjectId")));

        Assert.Null(await new LocalAppUserResolver(mediator.Object).Resolve(ContextFor(Oid)));
    }

    [Fact]
    public async Task A_failing_user_lookup_is_an_error_not_an_unknown_user()
    {
        // A database or handler failure must surface as an error; "unknown user" would be cached and answer 401 to everyone
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<ValidateUserAccessCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("An error occurred while processing the request.", new TimeoutException("MongoDB")));

        await Assert.ThrowsAsync<InvalidOperationException>(() => new LocalAppUserResolver(mediator.Object).Resolve(ContextFor(Oid)));
    }

    [Fact]
    public async Task Linking_the_same_entra_id_again_is_not_a_refusal()
    {
        // Several services resolve a new user at once on first sign-in; a later one sees the account already linked to this id
        var users = new Mock<IAppUserRepository>();
        users.Setup(r => r.GetUserByEmail("ada@example.org")).ReturnsAsync(User(entraObjectId: Oid));
        var handler = new OAuth2ConnectHandler(null!, NullLogger<OAuth2ConnectHandler>.Instance, users.Object);

        await handler.Handle(new OAuth2ConnectCommand { Email = "ada@example.org", EntraObjectId = Oid }, default);

        users.Verify(r => r.UpdateUser(It.Is<AppUser>(u => u.EntraObjectId == Oid)), Times.Once);
    }

    [Fact]
    public async Task Linking_tolerates_the_same_account_being_linked_concurrently()
    {
        // Stale read: this account shows no Entra id yet, but the uniqueness lookup finds this same account already linked
        var account = User(entraObjectId: null);
        var alreadyLinked = User(entraObjectId: Oid);
        alreadyLinked.Id = account.Id;
        var users = new Mock<IAppUserRepository>();
        users.Setup(r => r.GetUserByEmail("ada@example.org")).ReturnsAsync(account);
        users.Setup(r => r.GetUserByEntraObjectId(Oid)).ReturnsAsync(alreadyLinked);
        var handler = new OAuth2ConnectHandler(null!, NullLogger<OAuth2ConnectHandler>.Instance, users.Object);

        await handler.Handle(new OAuth2ConnectCommand { Email = "ada@example.org", EntraObjectId = Oid }, default);

        // Saved with the id in place, never with the stale null
        users.Verify(r => r.UpdateUser(It.Is<AppUser>(u => u.Id == account.Id && u.EntraObjectId == Oid)), Times.Once);
    }

    private static DefaultHttpContext ContextFor(string oid) => new()
    {
        User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimConstants.ObjectId, oid) }, "Bearer")),
    };
}

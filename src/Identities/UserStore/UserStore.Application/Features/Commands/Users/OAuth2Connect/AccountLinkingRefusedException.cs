namespace UserStore.Application.Features.Commands.Users.OAuth2Connect
{
    // The token's identity cannot be linked to the account found by its email (already linked elsewhere, or the
    // account is unknown): that caller is not a valid user. Anything else thrown while validating a user is an error.
    public class AccountLinkingRefusedException : InvalidOperationException
    {
        public AccountLinkingRefusedException(string message) : base(message) { }
    }
}

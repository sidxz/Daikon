
/* 
== Overview
The UserStoreAPIService class is part of the Ocelot API Gateway infrastructure and is responsible for validating
user access by interacting with an external User Store API. This service is designed to send user details to 
the User Store API and interpret the response to determine if the user is authorized.

== Parameters:
string oidcSub: Optional. The OIDC subject identifier.
string entraObjectId: Optional. The Entra (formerly known as Microsoft) object identifier.
string email: The user's email address.
*/
namespace OcelotApiGw.Contracts.Infrastructure
{
    public partial class UserStoreAPIService : IUserStoreAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserStoreAPIService> _logger;
        private readonly string _userStoreApiBaseUrl;
        private readonly string _userStoreAPIValidateUrl;
        private readonly string _userStoreAPIResolvePermissionUrl;

        public UserStoreAPIService(ILogger<UserStoreAPIService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userStoreApiBaseUrl = configuration.GetSection("UserStoreAPI:BaseUrl")?.Value ?? throw new ArgumentNullException(nameof(_userStoreApiBaseUrl));
            _userStoreAPIValidateUrl = configuration.GetSection("UserStoreAPI:Validate")?.Value ?? throw new ArgumentNullException(nameof(_userStoreAPIValidateUrl));
            _userStoreAPIResolvePermissionUrl = configuration.GetSection("UserStoreAPI:ResolvePermission")?.Value ?? throw new ArgumentNullException(nameof(_userStoreAPIValidateUrl));

        }


        
    }
}
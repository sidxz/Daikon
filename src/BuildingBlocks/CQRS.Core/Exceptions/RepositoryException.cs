
namespace CQRS.Core.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string repositoryName, string message, Exception innerException)
            : base($"RepositoryException at: {repositoryName} : {message}", innerException)
        {
        }
    }
}
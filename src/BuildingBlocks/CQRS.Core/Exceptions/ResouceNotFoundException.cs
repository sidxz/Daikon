namespace CQRS.Core.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        // Constructor that accepts a string ID
        public ResourceNotFoundException(string resourceName, string id) 
            : base($"ResourceNotFoundException: The requested resource {resourceName} with identifier {id} was not found")
        {
        }

        // Constructor that accepts a Guid ID
        public ResourceNotFoundException(string resourceName, Guid id) 
            : base($"ResourceNotFoundException: The requested resource {resourceName} with Guid {id} was not found")
        {
        }
    }
}

namespace Target.Domain.Exceptions
{
    public class DomainInvariantViolationException(string message) : Exception(message)
    {
    }
}

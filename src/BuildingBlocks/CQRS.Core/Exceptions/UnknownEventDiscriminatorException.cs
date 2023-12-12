
namespace CQRS.Core.Exceptions
{
    public class UnknownEventDiscriminatorException : Exception
    {
        public UnknownEventDiscriminatorException(string message) : base(message)
        {
        }
    }
}
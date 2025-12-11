namespace Target.Domain.Services
{
    public interface ITargetUniquenessChecker
    {
        Task EnsureTargetNameIsUniqueAsync(Guid strainId, string name, Guid? existingTargetId = null);
    }
}

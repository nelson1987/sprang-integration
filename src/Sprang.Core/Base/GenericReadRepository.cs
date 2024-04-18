namespace Sprang.Core.Base;

public interface IGenericReadRepository<T> where T : AuditableEntity
{
}

public class GenericReadRepository<T> : IGenericReadRepository<T> where T : AuditableEntity
{
}
namespace Sprang.Core.Base;

public interface IGenericWriteRepository<T> where T : AuditableEntity
{
}

public class GenericWriteRepository<T> : IGenericWriteRepository<T> where T : AuditableEntity
{
}

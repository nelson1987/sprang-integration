namespace Sprang.Core.Base;

public abstract class AuditableEntity
{
    public Guid Id { get; set; }
    public DateTime Criacao { get; set; }
}

namespace Sprang.Core.Base;

public abstract record AuditableEvent
{
    public Guid Id { get; set; }
    public DateTime Criacao { get; set; }
}

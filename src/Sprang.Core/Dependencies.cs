using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Sprang.Core;

public static class Dependencies
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IMovimentacaoHandler, MovimentacaoHandler>();
        services.AddScoped<IValidator<MovimentacaoCommand>, SolicitacaoValidator>();
        services.AddScoped(typeof(IGenericWriteRepository<>), typeof(GenericWriteRepository<>));
        services.AddScoped(typeof(IGenericReadRepository<>), typeof(GenericReadRepository<>));
        services.AddScoped(typeof(IGenericProducer<>), typeof(GenericProducer<>));
        //IGenericProducer
        return services;
    }
}

public interface IMovimentacaoHandler
{
    Task Handle(MovimentacaoCommand command, CancellationToken cancellationToken = default);
}

public class MovimentacaoHandler : IMovimentacaoHandler
{
    private readonly IValidator<MovimentacaoCommand> _validator;
    private readonly IGenericWriteRepository<ContaBancaria> _contaBancariaWriteRepository;
    private readonly IGenericProducer<MovimentacaoCriadaEvent> _movimentacaoCriadaEvent;
    public MovimentacaoHandler(IValidator<MovimentacaoCommand> validator, 
        IGenericWriteRepository<ContaBancaria> contaBancariaWriteRepository, 
        IGenericProducer<MovimentacaoCriadaEvent> movimentacaoCriadaEvent)
    {
        _validator = validator;
        _contaBancariaWriteRepository = contaBancariaWriteRepository;
        _movimentacaoCriadaEvent = movimentacaoCriadaEvent;
    }

    public async Task Handle(MovimentacaoCommand command, CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new NotImplementedException();
        }

        await Task.CompletedTask;
    }
}

public abstract class AuditableEntity
{
    public Guid Id { get; set; }
    public DateTime Criacao { get; set; }
}

public class ContaBancaria : AuditableEntity
{
    public string Numero { get; set; }
}

public class Movimentacao : AuditableEntity
{
    public ContaBancaria Debitante { get; set; }
    public ContaBancaria Creditante { get; set; }
    public decimal Valor { get; set; }
    public TipoSolicitacao Tipo { get; set; }
}

public record MovimentacaoCommand(string Creditante, decimal Valor, TipoSolicitacao Tipo);

public abstract record AuditableEvent
{
    public Guid Id { get; set; }
    public DateTime Criacao { get; set; }
}

public record MovimentacaoCriadaEvent(string Debitante, string Creditante, decimal Valor, TipoSolicitacao Tipo) : AuditableEvent;

public enum TipoSolicitacao
{
    Transferencia = 1
}

public class SolicitacaoValidator : AbstractValidator<MovimentacaoCommand>
{
    public SolicitacaoValidator()
    {
        RuleFor(x => x.Creditante).NotEmpty();
        RuleFor(x => x.Valor).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Tipo).IsInEnum();
    }
}

public interface IGenericWriteRepository<T> where T : AuditableEntity
{
}

public class GenericWriteRepository<T> : IGenericWriteRepository<T> where T : AuditableEntity
{
}

public interface IGenericProducer<T> where T : AuditableEvent
{
}

public class GenericProducer<T> : IGenericProducer<T> where T : AuditableEvent
{
}

public interface IGenericReadRepository<T> where T : AuditableEntity
{
}

public class GenericReadRepository<T> : IGenericReadRepository<T> where T : AuditableEntity
{
}
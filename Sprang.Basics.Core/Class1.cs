using Sprang.Basics.Core.Extensions;

namespace Sprang.Basics.Core;

public interface IFuncionaroHandler
{
    Task<Result> Handle(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    IRepository<Funcionario> Funcionarios { get; set; }
    IRepository<Gerente> Gerentes { get; set; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

public interface IRepository<TEvent> where TEvent : class
{
    Task InsertAsync(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IEventBus<TEvent> where TEvent : class
{
    Task Send(TEvent @event, CancellationToken cancellationToken = default);
}

public record InclusaoFuncionarioCommand(string Nome);

public class InclusaoContaCommandValidator : AbstractValidator<InclusaoFuncionarioCommand>
{
    public InclusaoContaCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty();
    }
}

public record FuncionarioIncluidoEvent(string Nome);

public record Funcionario(string Nome);

public record Gerente(string Nome);

public class FuncionaroHandler : IFuncionaroHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus<FuncionarioIncluidoEvent> _eventBus;
    private readonly IValidator<InclusaoFuncionarioCommand> _validator;

    public FuncionaroHandler(IUnitOfWork unitOfWork,
                            IEventBus<FuncionarioIncluidoEvent> eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<Result> Handle(CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new InclusaoFuncionarioCommand("Nome");
            var result = _validator.Validate(command);
            if (result != null)
                return result.ToFailResult();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.Funcionarios.InsertAsync(command.MapTo<Funcionario>(), cancellationToken);
            await _unitOfWork.Gerentes.InsertAsync(command.MapTo<Gerente>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _eventBus.Send(command.MapTo<FuncionarioIncluidoEvent>(), cancellationToken);

        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Fail(e.Message);
        }
        return Result.Ok();
    }
}

using Sprang.Basics.Core.Extensions;

namespace Sprang.Basics.Core.Generics;

public interface IGenericHandler<TResponse, TCommand>
    where TResponse : class
    where TCommand : class
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface IGenericHandler<TResponse, TCommand, TEvent>
    where TResponse : class
    where TCommand : class
    where TEvent : class
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
    Task Publish(TEvent @event, CancellationToken cancellationToken = default);
}

public abstract class GenericHandler<TResponse, TCommand, TEvent> :
    IGenericHandler<TResponse, TCommand, TEvent>
    where TResponse : class
    where TCommand : class
    where TEvent : class

{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IEventBus<TEvent> _eventBus;
    protected readonly IValidator<TCommand> _validator;

    public async Task Publish(TEvent @event, CancellationToken cancellationToken = default)
    {
        await _eventBus.Send(@event, cancellationToken);
    }

    public abstract Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);

}

public interface IFuncionarioHandler : IGenericHandler<Result, BaseCommand, BaseEvent>
{
}

public class FuncionarioHandler : GenericHandler<Result, BaseCommand, BaseEvent>, IFuncionarioHandler
{
    public override async Task<Result> Handle(BaseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _validator.ValidateAsync(command, cancellationToken);
            if (result != null)
                return await result.ToFailResult();

            await Persistence(command, cancellationToken);
            await Publish(command.MapTo<BaseEvent>(), cancellationToken);
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Fail(e.Message);
        }
        return Result.Ok();
    }

    private async Task Persistence(BaseCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _unitOfWork.Funcionarios.InsertAsync(command.MapTo<Funcionario>(), cancellationToken);
        await _unitOfWork.Gerentes.InsertAsync(command.MapTo<Gerente>(), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}


public interface IBaseResponse
{
}

public abstract class BaseResponse : IBaseResponse
{
}

public interface IBaseCommand
{
}

public abstract class BaseCommand : IBaseCommand
{
}

public interface IBaseEvent
{
}

public abstract class BaseEvent : IBaseEvent
{
}
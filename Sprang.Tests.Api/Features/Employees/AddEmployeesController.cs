using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Sprang.Tests.Api.Extensions;

namespace Sprang.Tests.Api.Features.Employees;

[Route("api/employees")]
public class AddEmployeesController : ApiControllerBase
{
    private readonly IAddEmployeeCommandHandler _handler;
    private readonly IValidator<AddEmployeeCommand> _validator;
    public AddEmployeesController() : base()
    {

    }
    [HttpPost]
    public async Task<ActionResult> Post(AddEmployeeCommand command, CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Post");
        var validationResult = _validator.Validate(command);
        if (validationResult.IsInvalid())
            return UnprocessableEntity(validationResult.ToModelState());

        var result = await _handler.Handle(command, cancellationToken);
        return result.IsFailed
            ? BadRequest()
            : Created();
    }
}
public record AddEmployeeCommand();

public interface IAddEmployeeCommandHandler
{
    Task<Result> Handle(AddEmployeeCommand command, CancellationToken cancellationToken);
}

public class AddEmployeeCommandHandler : IAddEmployeeCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddEmployeeCommand command, CancellationToken cancellationToken)
    {

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _unitOfWork.Employees.InsertAsync(cancellationToken);
        await _unitOfWork.Managers.InsertAsync(cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        //throw new NotImplementedException();

        //Buscar as contas a serem debitadas e creditadas
        //Persistir os dados
        //Publicar Transferencia
        //var autorizado = await _httpExternalServiceClient.GetTaskAsync(cancellationToken);
        //if (!autorizado) return Result.Fail("Não Autorizado.");

        //Movement aluno = command;
        //await _writeRepository.CreateAsync(aluno, cancellationToken);

        //MovementEvent @event = aluno;
        //await _producer.Send(@event, cancellationToken);
        return Result.Ok();
    }
}
public static class Dependencies
{
    public static IServiceCollection AddEmployeesFeatures(this IServiceCollection services)
    {
        services.AddServices();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAddEmployeeCommandHandler, AddEmployeeCommandHandler>();
        //services.AddScoped<IUnitOfWork, UserRepository>();
        //services.AddScoped<IRepository, UserRepository>();
        return services;
    }
}
using FluentValidation;
using Sprang.Core.Base;
using Sprang.Core.Features.ContasBancarias;

namespace Sprang.Core.Features.Movimentacoes;

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

        await _movimentacaoCriadaEvent.Send(
            new MovimentacaoCriadaEvent(
                "Debitante",
                command.Creditante,
                command.Valor,
                command.Tipo));

        await Task.CompletedTask;
    }
}

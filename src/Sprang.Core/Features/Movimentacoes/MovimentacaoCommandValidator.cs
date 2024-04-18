using FluentValidation;

namespace Sprang.Core.Features.Movimentacoes;

public class MovimentacaoCommandValidator : AbstractValidator<MovimentacaoCommand>
{
    public MovimentacaoCommandValidator()
    {
        RuleFor(x => x.Creditante).NotEmpty();
        RuleFor(x => x.Valor).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Tipo).IsInEnum();
    }
}

using Sprang.Core.Features.Solicitacoes;

namespace Sprang.Core.Features.Movimentacoes;

public record MovimentacaoCommand(string Creditante, decimal Valor, TipoSolicitacao Tipo);

using Sprang.Core.Base;
using Sprang.Core.Features.Solicitacoes;

namespace Sprang.Core.Features.Movimentacoes;

public record MovimentacaoCriadaEvent(string Debitante, string Creditante, decimal Valor, TipoSolicitacao Tipo) : AuditableEvent;

using Sprang.Core.Base;
using Sprang.Core.Features.ContasBancarias;
using Sprang.Core.Features.Solicitacoes;

namespace Sprang.Core.Features.Movimentacoes;

public class Movimentacao : AuditableEntity
{
    public ContaBancaria Debitante { get; set; }
    public ContaBancaria Creditante { get; set; }
    public decimal Valor { get; set; }
    public TipoSolicitacao Tipo { get; set; }
}

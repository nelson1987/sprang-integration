using Microsoft.AspNetCore.Mvc;
using Sprang.Core.Features.Movimentacoes;

namespace Sprang.Api.Features;

[Route("api/[Controller]")]
public class MovimentacoesController : ApiControllerBase
{

    //[FeatureGate(FeatureFlags.Santa)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> GetSantaCar(
        [FromServices] IMovimentacaoHandler handler,
        [FromBody] MovimentacaoCommand command, 
        CancellationToken cancellationToken = default)
    {
        await handler.Handle(command, cancellationToken);
        return Created();
    }
}

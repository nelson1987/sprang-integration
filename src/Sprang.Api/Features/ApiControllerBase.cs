using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Sprang.Api.Features;

[Produces("application/json")]
[ApiController]
[Route("api/[Controller]")]
public abstract class ApiControllerBase : ControllerBase
{

}
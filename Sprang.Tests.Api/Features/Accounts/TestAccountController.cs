using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sprang.Tests.Api.Features.Accounts;

[Route("api/testes")]
public class TestesController : ApiControllerBase
{
    public TestesController() 
        : base()
    {

    }

    [HttpGet]
    [Route("anonymous")]
    [AllowAnonymous]
    public async Task<string> Anonymous(CancellationToken cancellationToken)
    {
        return "Anônimo";
    }

    [HttpGet]
    [Route("authenticated")]
    [Authorize]
    public async Task<string> Authenticated(CancellationToken cancellationToken)
    {
        var identity = (ClaimsIdentity?)User.Identity!;
        var roles = identity.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

        return $"Autenticado - {identity.Name} Role: {string.Join(",", roles.ToList())}";
    }

    [HttpGet]
    [Route("employee")]
    [Authorize(Roles = "employee,manager")]
    public async Task<string> Employee(CancellationToken cancellationToken)
    {
        var identity = (ClaimsIdentity?)User.Identity!;
        return $"Funcionário{identity.Name}";
    }

    [HttpGet]
    [Route("manager")]
    [Authorize(Roles = "manager")]
    public async Task<string> Manager(CancellationToken cancellationToken)
    {
        var identity = (ClaimsIdentity?)User.Identity!;
        return $"Gerente{identity.Name}";
    }
}
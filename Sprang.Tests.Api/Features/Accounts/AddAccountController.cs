using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sprang.Tests.Api.Features.Accounts;

[Route("api/accounts")]
public class AddAccountController : ApiControllerBase
{
    public AddAccountController() : base()
    {

    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public ActionResult<UserResponse> Authenticate([FromServices] IUserRepository userRepository, [FromServices] ITokenService tokenService, [FromBody] User model)
    {
        var user = userRepository.Get(model.Username, model.Password);

        if (user == null)
            return NotFound(new { message = "Usuário ou senha inválidos" });

        var token = tokenService.GenerateToken(user);

        return new UserResponse(user.Id, user.Username, user.Role, token);
    }

}
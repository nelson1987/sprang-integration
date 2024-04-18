using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Sprang.Tests.Api.Features;

[Produces("application/json")]
[ApiController]
[Route("api/[Controller]")]
public abstract class ApiControllerBase : ControllerBase
{
}
public record User(int Id, string Username, string Password, string Role);
public record UserResponse(int Id, string Username, string Role, string token);

public interface IUserRepository
{
    User? Get(string username, string password);
}

public interface ITokenService
{
    string GenerateToken(User user);
}
public class UserRepository : IUserRepository
{
    public User? Get(string username, string password)
    {
        var users = new List<User>
        {
            new(Id: 1, Username: "batman", Password: "batman", Role: "manager"),
            new(Id: 2, Username: "robin", Password: "robin", Role: "employee")
        };
        return users.FirstOrDefault(x => x.Username.ToLower() == username.ToLower() && x.Password == password);
    }
}

public class TokenService : ITokenService
{
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public static class Settings
{
    public static readonly string Secret = "fedaf7d8863b48e197b9287d492b708e";
}

public static class Dependencies
{
    public static IServiceCollection AddUserFeatures(this IServiceCollection services)
    {
        services.AddServices()
            .AddUserAuthentication();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }

    private static IServiceCollection AddUserAuthentication(this IServiceCollection services)
    {
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        return services;
    }
}
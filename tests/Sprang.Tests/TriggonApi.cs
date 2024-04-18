using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Sprang.Tests;

public class TriggonApi : WebApplicationFactory<Program>
{
    static TriggonApi()
        => Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseEnvironment("Test")
            .ConfigureTestServices(services =>
            {
            });
}

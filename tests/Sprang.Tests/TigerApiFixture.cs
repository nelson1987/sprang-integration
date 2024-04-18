using System.Net.Http.Headers;

namespace Sprang.Tests;

public class TigerApiFixture
{
    private static readonly TriggonApi _server;
    private static readonly HttpClient _client;

    public TriggonApi Server => _server;
    public HttpClient Client => _client;
    static TigerApiFixture()
    {
        _server = new();
        _client = _server.CreateDefaultClient();
    }
    public TigerApiFixture()
    {
        _client.DefaultRequestHeaders.Clear();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");
    }
}

using AutoFixture;
using AutoFixture.AutoMoq;
using Sprang.Core.Features.Movimentacoes;
using System.Text;

namespace Sprang.Tests;

public class MovimentacaoControllerIntegrationTests : IntegrationTests, IClassFixture<TigerApiFixture>
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TigerApiFixture _tigerApiFixture;
    private MovimentacaoCommand _command => _fixture.Build<MovimentacaoCommand>().Create();

    public MovimentacaoControllerIntegrationTests(TigerApiFixture tigerApiFixture)
    {
        _tigerApiFixture = tigerApiFixture;
    }

    [Fact]
    public async Task Dado_Requisicao_Valida_Retorna_Created()
    {
        // Act
        var updateFinancialDataAsString = System.Text.Json.JsonSerializer.Serialize(_command);
        using var stringContent = new StringContent(updateFinancialDataAsString, Encoding.UTF8, "application/json");
        await _tigerApiFixture.Client.PostAsync("/movimentacoes", stringContent);

        //var products = MongoDbFixture.Produtos();
        //Assert.Equal(products.Count, 1);

        //var consumed = RabbitMqFixture.Produtos();
        //Assert.Equal(consumed.Count, 1);
    }
}

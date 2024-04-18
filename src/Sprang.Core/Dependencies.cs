using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sprang.Core.Base;
using Sprang.Core.Features.Movimentacoes;

namespace Sprang.Core;

public static class Dependencies
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IMovimentacaoHandler, MovimentacaoHandler>();
        services.AddScoped<IValidator<MovimentacaoCommand>, MovimentacaoCommandValidator>();
        services.AddScoped(typeof(IGenericWriteRepository<>), typeof(GenericWriteRepository<>));
        services.AddScoped(typeof(IGenericReadRepository<>), typeof(GenericReadRepository<>));
        services.AddScoped(typeof(IGenericProducer<>), typeof(GenericProducer<>));
        //IGenericProducer
        return services;
    }
}

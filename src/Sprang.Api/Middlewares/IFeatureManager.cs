using Sprang.Api.Features;
using Sprang.Core.Features.Cars;

namespace Sprang.Api.Middlewares;

public interface IFeatureManager
{
    Task<IEnumerable<GetCarQuery>> IsEnabledAsync(CancellationToken cancellationToken = default);
}

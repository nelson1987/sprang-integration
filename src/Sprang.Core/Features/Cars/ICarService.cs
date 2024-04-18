namespace Sprang.Core.Features.Cars;

public interface ICarService
{
    Task<IEnumerable<GetCarQuery>> GetAllSortedByPlateAsync(CancellationToken cancellationToken = default);
}

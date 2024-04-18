using Microsoft.AspNetCore.Mvc;
using Sprang.Core.Features.Cars;


namespace Sprang.Api.Features;

public class CarsController : ApiControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetCarQuery>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _carService.GetAllSortedByPlateAsync(cancellationToken);
        return Ok(result);
    }

    //[FeatureGate(FeatureFlags.Santa)]
    [HttpGet("santa")]
    [ProducesResponseType(typeof(GetCarQuery), StatusCodes.Status200OK)]
    public IActionResult GetSantaCar()
    {
        return Ok(new GetCarQuery(1, "Magic Sleigh", "XMas 12"));
    }
}

using Microsoft.AspNetCore.Mvc;
using Sprang.Core;

namespace Sprang.Api.Features;

[Route("api/[Controller]")]
public class MovimentacoesController : ApiControllerBase
{

    //[FeatureGate(FeatureFlags.Santa)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> GetSantaCar([FromServices] IMovimentacaoHandler handler, [FromBody] MovimentacaoCommand command, CancellationToken cancellationToken = default)
    {
        await handler.Handle(command, cancellationToken);
        return Created();
    }
}
public interface ICarService
{
    Task<IEnumerable<GetCarQuery>> GetAllSortedByPlateAsync(CancellationToken cancellationToken = default);
}
public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDetailsDto> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> InsertAsync(EmployeePostDto dto, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateAsync(int id, EmployeePutDto dto, CancellationToken cancellationToken = default);
    Task<EmployeeDto> GetOldestAsync(CancellationToken cancellationToken = default);
}
public interface IFeatureManager
{
    Task<IEnumerable<GetCarQuery>> IsEnabledAsync(CancellationToken cancellationToken = default);
}

public record GetCarQuery(int Id, string Model, string Plate);
public record EmployeeDto(int Id, string FirstName, string LastName, string Gender, DateTime BirthDate);
public record EmployeeDetailsDto(int Id, string Model, string Plate);
public record EmployeePostDto(int Id, string Model, string Plate);
public record EmployeePutDto(int Id, string Model, string Plate);

[Produces("application/json")]
[ApiController]
[Route("api/[Controller]")]
public abstract class ApiControllerBase : ControllerBase
{

}

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
        return Ok(new GetCarQuery(int.MaxValue, "Magic Sleigh", "XMas 12"));
    }
}

public class EmployeesController : ApiControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IFeatureManager _featureManager;

    public EmployeesController(IEmployeeRepository employeeRepository, IFeatureManager featureManager)
    {
        _employeeRepository = employeeRepository;
        _featureManager = featureManager;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _employeeRepository.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(EmployeeDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("oldest")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOldestAsync(
        CancellationToken cancellationToken = default)
    {
        //if (await _featureManager.IsEnabledAsync(FeatureFlags.Santa))
        //{
        //return Ok(new EmployeeDto
        //{
        //    Id = int.MaxValue,
        //    FirstName = "Santa",
        //    LastName = "Claus",
        //    BirthDate = new DateTime(270, 3, 15),
        //    Gender = "M",
        //});
        //}

        var result = await _employeeRepository.GetOldestAsync(cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] EmployeePutDto employeePutDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeRepository.UpdateAsync(id, employeePutDto, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync(
        [FromBody] EmployeePostDto employeePostDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeRepository.InsertAsync(employeePostDto, cancellationToken);
        Response.Headers.Append("x-date-created", DateTime.UtcNow.ToString("s"));
        return CreatedAtAction("Get", new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeRepository.DeleteByIdAsync(id, cancellationToken);
        if (result)
        {
            return NoContent();
        }
        return NotFound();
    }
}
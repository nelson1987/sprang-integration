namespace Sprang.Core.Features.Employees;

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

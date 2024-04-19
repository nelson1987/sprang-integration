using Sprang.Tests.Api.Features.Employees;

namespace Sprang.Tests.Api.Repositories;
/*
public record Employee();
public record Manager();
public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
public class Transaction : ITransaction
{
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
public interface IDatabase
{
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
public interface IDatabaseContext : IDatabase
{
    Task<IQueryable<TEntity>> GetEntitySet<TEntity>(CancellationToken cancellationToken = default) where TEntity : class;
    Task Add<TEntity>(TEntity order, CancellationToken cancellationToken = default) where TEntity : class;
}
public class DummyDatabase : IDatabaseContext
{
    private readonly List<Employee> _orders = new List<Employee>();

    public Task<IQueryable<TEntity>> GetEntitySet<TEntity>(CancellationToken cancellationToken = default) where TEntity : class => _orders.AsQueryable() as Task<IQueryable<TEntity>>;

    public Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
        //return Task.FromResult(new EfTransaction(null) as ITransaction);
    }

    public Task Add<TEntity>(TEntity order, CancellationToken cancellationToken = default) where TEntity : class
    {
        _orders.Add(order as Employee);
    }
}
public interface IUnitOfWork
{
    IRepository<Employee> Employees { get; set; }
    IRepository<Manager> Managers { get; set; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly IDatabase _database;
    private ITransaction? _currentTransaction;
    public UnitOfWork(IDatabase database)
    {
        _database = database;
    }
    public IRepository<Employee> Employees { get; set; }
    public IRepository<Manager> Managers { get; set; }
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException("A transaction has already been started.");
        _currentTransaction = await _database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("A transaction has not been started.");

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
        catch (Exception)
        {
            if (_currentTransaction is not null)
                await _currentTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

public interface IRepository<TEntity> where TEntity : class
{
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public abstract class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly IDatabaseContext _database;

    protected GenericRepository(IDatabaseContext database)
    {
        _database = database;
    }

    public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _database.Add(entity,cancellationToken);
    }
}
*/
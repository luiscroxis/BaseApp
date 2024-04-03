namespace BaseApp.Infra.Repository.Orm.UnitOfWork;

using Contexts;
using Domain.Repository.Orm.Abstract.Contexts;
using Domain.Repository.Orm.Abstract.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    protected IDbContext _context;
    protected IDbContextTransaction _transaction;

    public UnitOfWork(DbContext context) => _context = context;

    public async Task<int> SaveChanges(CancellationToken cancellationToken = default) => await _context.SaveChangeAsync(cancellationToken);

    public virtual void Commit()
    {
        if (_transaction == null)
        {
            return;
        }

        try
        {
            _context.SaveChangeAsync();
            _transaction.Commit();
            _transaction = null;
        }
        catch
        {
            Rollback();
            throw;
        }
    }

    public virtual void Rollback()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction = null;
        }
    }

    public void AddContext(IDbContext dataContext) => _context = dataContext;

    public virtual void OpenTransaction()
    {
        if (_transaction == null)
        {
            _transaction = ((DbContext)_context).Database.BeginTransaction();
        }
    }

    public void Dispose() => _transaction?.Dispose();
}

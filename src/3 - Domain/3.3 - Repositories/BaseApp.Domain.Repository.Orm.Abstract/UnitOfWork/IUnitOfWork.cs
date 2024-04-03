namespace BaseApp.Domain.Repository.Orm.Abstract.UnitOfWork;

using Contexts;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChanges(CancellationToken cancellationToken = default);
    void OpenTransaction();
    void Commit();
    void Rollback();
    void AddContext(IDbContext dataContext);
}

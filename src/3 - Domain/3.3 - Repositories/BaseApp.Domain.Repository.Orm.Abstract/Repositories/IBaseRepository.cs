namespace BaseApp.Domain.Repository.Orm.Abstract.Repositories;

using System.Linq.Expressions;
using Entity.Bases;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query { get; }
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetByAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
    Task<PaginatedEntity<TEntity>> GetPaginatedAsync(string colunmSort, Expression<Func<TEntity, bool>> by, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken);
    Task<PaginatedEntity<TEntity>> GetPaginatedAsync(string colunmSort, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes);
    Task<PaginatedEntity<TEntity>> GetPaginatedAsync(string colunmSort, Expression<Func<TEntity, bool>>? by, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includes);
    //Task<PaginatedUserEntity> GetUserPaginatedAsync(IQueryable<User> query, Expression<Func<User, bool>>? by, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken, params Expression<Func<User, object>>[]? includes);
    Task<TEntity?> GetByOneAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByOneAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] columns);
    Task BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] columns);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
   // Task DeleteUserClaimsAsync(UserClaims entity, CancellationToken cancellationToken = default);
    Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default);
    Task<Expression<Func<TEntity, bool>>> FilterExpression<T>(T model);
}

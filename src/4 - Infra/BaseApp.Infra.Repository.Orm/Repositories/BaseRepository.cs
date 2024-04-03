namespace BaseApp.Infra.Repository.Orm.Repositories;

using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using Domain.Entity.Bases;
using Domain.Repository.Orm.Abstract.Contexts;
using Domain.Repository.Orm.Abstract.Repositories;
using Domain.Repository.Orm.Abstract.UnitOfWork;
using Extensions;
using Filter;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    public IQueryable<TEntity> Query => _context.Set<TEntity>().AsQueryable();

    public BaseRepository(IDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => GetByIdAsync(id, cancellationToken, null);

    public Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includes)
    {
        var entity = _context.Set<TEntity>();
        if ((includes?.Length ?? 0) <= 0)
            return entity.FindAsync(id).AsTask();
        foreach (var inc in includes!)
            entity.Include(inc);
        return entity.FindAsync(id, cancellationToken).AsTask();
    }

    public Task<IEnumerable<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default) => GetByAsync(by, cancellationToken, null);

    public Task<IEnumerable<TEntity>> GetByAsync(CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes) =>
        GetByAsync(null, cancellationToken, includes);

    public async Task<IEnumerable<TEntity>> GetByAsync(Expression<Func<TEntity, bool>>? by, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includes)
    {
        var query = Query;

        if (includes?.Length > 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (by != null)
            query = query.Where(by);

        var data = await query.AsNoTracking().ToListAsync(cancellationToken);
        return data;
    }

     public Task<PaginatedEntity<TEntity>> GetPaginatedAsync(string colunmSort, Expression<Func<TEntity, bool>> by, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken) =>
         GetPaginatedAsync(colunmSort, by, metaData, cancellationToken, null);

     public Task<PaginatedEntity<TEntity>> GetPaginatedAsync(string colunmSort, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken,
         params Expression<Func<TEntity, object>>[] includes) =>
         GetPaginatedAsync(colunmSort, null, metaData, cancellationToken, includes);

     public async Task<PaginatedEntity<TEntity>> GetPaginatedAsync(string colunmSort, Expression<Func<TEntity, bool>>? by, PaginatedMetaDataEntity metaData, CancellationToken cancellationToken,
          params Expression<Func<TEntity, object>>[]? includes)
     {
         var query = Query;

         if (includes?.Length > 0)
             query = includes.Aggregate(query, (current, include) => current.Include(include.AsPath()));

         if (by != null)
             query = query.Where(by);

         if (!string.IsNullOrEmpty(colunmSort))
         {
             var param = Expression.Parameter(typeof(TEntity));
             var memberAccess = Expression.Property(param, colunmSort.Replace("-", ""));
             var convertedMemberAccess = Expression.Convert(memberAccess, typeof(object));
             var orderPredicate = Expression.Lambda<Func<TEntity, object>>(convertedMemberAccess, param);

             if (colunmSort.Contains("-"))
             {
                 query = query.OrderByDescending(orderPredicate);
             }
             else
             {
                 query = query.OrderBy(orderPredicate);
             }
         }

         var count = await query.AsNoTracking().CountAsync(cancellationToken);
         query = query.Skip((metaData.PageNumber - 1) * metaData.PageSize).Take(metaData.PageSize);
         var items = await query.AsNoTracking().ToListAsync(cancellationToken);
         var totalPages = (count / metaData.PageSize);

         if (totalPages <= 0 && count > 0)
             totalPages = 1;

         return new PaginatedEntity<TEntity>
         {
             MetaData = new PaginatedMetaDataEntity
             {
                 TotalItems = count,
                 TotalPages = totalPages,
                 PageSize = metaData.PageSize,
                 PageNumber = metaData.PageNumber
             },
             Items = items
         };
     }

     //public async Task<PaginatedUserEntity> GetUserPaginatedAsync(IQueryable<User> query,
     //    Expression<Func<User, bool>>? by,
     //    PaginatedMetaDataEntity metaData,
     //    CancellationToken cancellationToken,
     //    params Expression<Func<User, object>>[]? includes)
     //{

        // if (includes?.Length > 0)
         //    query = includes.Aggregate(query, (current, include) => current.Include(include.AsPath()));

         //if (by != null)
         //    query = query.Where(by);

         //var count = await query.AsNoTracking().CountAsync(cancellationToken);
         //query = query.Skip((metaData.PageNumber - 1) * metaData.PageSize).Take(metaData.PageSize);

         //var items = await query.AsNoTracking().ToListAsync(cancellationToken);
         //var totalPages = (count / metaData.PageSize);

         //if (totalPages <= 0 && count > 0)
         //    totalPages = 1;

         //return new PaginatedUserEntity()
         //{
         //    MetaData = new PaginatedMetaDataEntity
          //   {
           //      TotalItems = count,
           //      TotalPages = totalPages,
          //       PageSize = metaData.PageSize,
          //       PageNumber = metaData.PageNumber
          //   },
         //    Items = items
        // };
    // }

    public Task<TEntity?> GetByOneAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default) => GetByOneAsync(by, cancellationToken, null);

    public async Task<TEntity?> GetByOneAsync(Expression<Func<TEntity, bool>> by, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includes)
    {
        var query = Query;
        if (includes?.Length > 0)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        var data = await query.AsNoTracking().FirstOrDefaultAsync(by, cancellationToken);
        return data;
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }


        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChanges(cancellationToken);
        return entity;
    }

    public async Task BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public async Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] columns)
    {
        if (!entities.Any() || !columns.Any())
            throw new ArgumentException($"Caro desenvolvedor, os parâmetros: {nameof(entities)} e {nameof(columns)} são obrigatórios");

        var items = await Query.Where(x => entities.Any(a => a.Id.Equals(x.Id))).ToListAsync(cancellationToken);

        items.ForEach(item =>
        {
            foreach (var entity in entities)
            {
                if (columns == null)
                    continue;
                var itemDatabase = item.GetType();
                var itemEntity = entity.GetType();
                foreach (var column in columns)
                {
                    var propertyName = column.Body switch
                    {
                        MemberExpression expression => expression.Member.Name,
                        UnaryExpression { Operand: MemberExpression operand } => operand.Member.Name,
                        _ => string.Empty
                    };

                    var valueEntity = itemEntity.GetProperty(propertyName)?.GetValue(entity);
                    itemDatabase.GetProperty(propertyName)?.SetValue(item, valueEntity);
                }
            }
        });

        _context.Set<TEntity>().UpdateRange(items);

        await Task.CompletedTask;
    }

    public async Task BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var items = await Query.Where(x => entities.Any(a => a.Id.Equals(x.Id))).ToListAsync(cancellationToken);

        if (items.Any())
            return;

        _context.Set<TEntity>().RemoveRange(items);

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] columns)
    {
        if (entity == null || !columns.Any())
            throw new ArgumentException($"Caro desenvolvedor, os parâmetros: {nameof(entity)} e {nameof(columns)} são obrigatórios");

        var item = await Query.FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), cancellationToken);

        var itemDatabase = item!.GetType();
        var itemEntity = entity.GetType();

        foreach (var column in columns)
        {
            var propertyName = column.Body switch
            {
                MemberExpression expression => expression.Member.Name,
                UnaryExpression { Operand: MemberExpression operand } => operand.Member.Name,
                _ => string.Empty
            };

            var valueEntity = itemEntity.GetProperty(propertyName)?.GetValue(entity);
            itemDatabase.GetProperty(propertyName)?.SetValue(item, valueEntity);
        }

        _context.Entry(entity);

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var item = await Query.FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), cancellationToken);
        if (item == null)
            return;

        _context.Set<TEntity>().Remove(item);

        await Task.CompletedTask;
    }

    // public async Task DeleteUserClaimsAsync(UserClaims entity, CancellationToken cancellationToken = default)
    // {
    //     if (entity == null)
    //         return;

    //     _context.Set<UserClaims>().Remove(entity);

    //     await Task.CompletedTask;
    // }


    public async Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        var save = await _unitOfWork.SaveChanges(cancellationToken);
        return save == 0;
    }

     public async Task<Expression<Func<TEntity, bool>>> FilterExpression<T>(T model)
     {
         if (model == null)
         {
             return null;
         }

         Expression lastExpression = null;

         var operations = ExpressionFactory.GetOperators<TEntity>(model);
         foreach (var expression in operations.Ordered())
         {
             if (!expression.Criteria.CaseSensitive)
             {
                 expression.FieldToFilter = Expression.Call(expression.FieldToFilter,
                     typeof(string).GetMethods()
                         .First(m => m.Name == "ToUpper" && m.GetParameters().Length == 0));

                 expression.FilterBy = Expression.Call(expression.FilterBy,
                     typeof(string).GetMethods()
                         .First(m => m.Name == "ToUpper" && m.GetParameters().Length == 0));
             }

             var actualExpression = GetExpression<TEntity>(expression);

             if (expression.Criteria.UseNot)
             {
                 actualExpression = Expression.Not(actualExpression);
             }

             if (lastExpression == null)
             {
                 lastExpression = actualExpression;
             }
             else
             {
                 if (expression.Criteria.UseOr)
                     lastExpression = Expression.Or(lastExpression, actualExpression);
                 else
                     lastExpression = Expression.And(lastExpression, actualExpression);
             }
         }

         return lastExpression != null ? Expression.Lambda<Func<TEntity, bool>>(lastExpression, operations.ParameterExpression) : null;
     }

     private static Expression GetExpression<TEntity>(ExpressionParser expression)
     {

         switch (expression.Criteria.Operator)
         {
             case WhereOperator.Equals:
                 return Expression.Equal(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.NotEquals:
                 return Expression.NotEqual(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.GreaterThan:
                 return Expression.GreaterThan(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.LessThan:
                 return Expression.LessThan(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.GreaterThanOrEqualTo:
                 return Expression.GreaterThanOrEqual(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.LessThanOrEqualTo:
                 return Expression.LessThanOrEqual(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.Contains:
                 return ContainsExpression<TEntity>(expression);
             case WhereOperator.GreaterThanOrEqualWhenNullable:
                 return GreaterThanOrEqualWhenNullable(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.LessThanOrEqualWhenNullable:
                 return LessThanOrEqualWhenNullable(expression.FieldToFilter, expression.FilterBy);
             case WhereOperator.StartsWith:
                 return Expression.Call(expression.FieldToFilter,
                     typeof(string).GetMethods()
                         .First(m => m.Name == "StartsWith" && m.GetParameters().Length == 1),
                     expression.FilterBy);
             default:
                 return Expression.Equal(expression.FieldToFilter, expression.FilterBy);
         }
     }

    private static Expression LessThanOrEqualWhenNullable(Expression e1, Expression e2)
    {
        if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            e2 = Expression.Convert(e2, e1.Type);

        else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            e1 = Expression.Convert(e1, e2.Type);

        return Expression.LessThanOrEqual(e1, e2);
    }

    private static Expression GreaterThanOrEqualWhenNullable(Expression e1, Expression e2)
    {
        if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            e2 = Expression.Convert(e2, e1.Type);

        else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            e1 = Expression.Convert(e1, e2.Type);

        return Expression.GreaterThanOrEqual(e1, e2);
    }

    private static bool IsNullableType(Type t)
    {
        return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

     private static Expression ContainsExpression<TEntity>(ExpressionParser expression)
     {
         if (expression.Criteria.Property.IsPropertyACollection())
        {
             var methodToApplyContains = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                 .Single(x => x.Name == "Contains" && x.GetParameters().Length == 2)
                 .MakeGenericMethod(expression.FieldToFilter.Type);

             return Expression.Call(methodToApplyContains, expression.FilterBy, expression.FieldToFilter);
        }
         else
         {
             var methodToApplyContains = expression.FieldToFilter.Type.GetMethods()
                 .First(m => m.Name == "Contains" && m.GetParameters().Length == 1);

             return Expression.Call(expression.FieldToFilter, methodToApplyContains, expression.FilterBy);
         }
     }

}

namespace BaseApp.Domain.Entity.Bases;

public class PaginatedEntity<TEntity> where TEntity : BaseEntity
{
    public PaginatedMetaDataEntity MetaData { get; set; }
    public IEnumerable<TEntity> Items { get; set; }
}

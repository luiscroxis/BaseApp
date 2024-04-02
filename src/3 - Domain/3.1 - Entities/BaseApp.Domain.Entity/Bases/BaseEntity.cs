using System.ComponentModel.DataAnnotations;


namespace BaseApp.Domain.Entity.Bases;

public abstract class BaseEntity<TEntityId>
{
    [Key]
    public TEntityId Id { get; set; }
}

public abstract class BaseEntity : BaseEntity<string>
{
}

public abstract class BaseEntityAudit<TEntityId, TUserIdAudity> : BaseEntity<TEntityId>
{
    public TUserIdAudity UserIdCreated { get; set; }
    public TUserIdAudity UserIdUpdated { get; set; }

    public DateTime CreatedAt { get; }
    public DateTime UpdateAt { get; set; }

    protected BaseEntityAudit()
    {
        CreatedAt = DateTime.Now;
    }
}

public abstract class BaseEntityAudit<TUserIdAudity> : BaseEntityAudit<Guid, TUserIdAudity>
{
}

public abstract class BaseEntityAudit : BaseEntityAudit<Guid, Guid>
{
}

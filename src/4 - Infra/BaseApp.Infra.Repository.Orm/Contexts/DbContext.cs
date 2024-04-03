namespace BaseApp.Infra.Repository.Orm.Contexts;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Domain.Entity.Bases;
using Domain.Repository.Orm.Abstract.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

[ExcludeFromCodeCoverage]
public class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
{
    private IDbContextTransaction _transaction;
    public new DbSet<T> Set<T>() where T : BaseEntity => base.Set<T>();

    public new EntityState Entry<T>(T entity) where T : BaseEntity =>
        base.Entry(entity).State = EntityState.Modified;

    public DbContext()
    {
    }

    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (Debugger.IsAttached)
            optionsBuilder.LogTo(l =>
            {
                Console.WriteLine(l);
                Debug.WriteLine(l);
            });
    }

    public Task<int> SaveChangeAsync(CancellationToken cancellationToken = default) => base.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync()
    {
        _transaction = await Database.BeginTransactionAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var typesConfiguration = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(gi =>
                gi.IsGenericType &&
                gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ToList();

        foreach (var configurationInstance in typesConfiguration.Select(Activator.CreateInstance))
            modelBuilder.ApplyConfiguration((dynamic)configurationInstance!);

        var strings = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string));

        foreach (var property in strings)
        {
            if (property.GetMaxLength() == null)
                property.SetMaxLength(200);
        }
    }
}

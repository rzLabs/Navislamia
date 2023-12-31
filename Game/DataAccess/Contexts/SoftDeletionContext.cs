using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Navislamia.Game.DataAccess.Entities;
using Navislamia.Game.DataAccess.Extensions;

namespace Navislamia.Game.DataAccess.Contexts;

public abstract class SoftDeletionContext : DbContext
{
    protected SoftDeletionContext()
    {
    }

    protected SoftDeletionContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureSoftDeletionQueryFilter(modelBuilder);
    }

    public override int SaveChanges()
    {
        SetSoftDeleteStatus();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        SetSoftDeleteStatus();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private static void ConfigureSoftDeletionQueryFilter(ModelBuilder modelBuilder)
    {
        modelBuilder.SetQueryFilterOnAllEntities<ISoftDeletableEntity>(e => e.DeletedOn == null);
    }

    private void SetSoftDeleteStatus()
    {
        ChangeTracker.DetectChanges();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDeletableEntity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.DeletedOn = null;
                        break;

                    case EntityState.Modified:
                        entity.ModifiedOn = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entity.DeletedOn = DateTime.UtcNow;
                        break;
                    
                    case EntityState.Detached:
                        break;
                    
                    case EntityState.Unchanged:
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
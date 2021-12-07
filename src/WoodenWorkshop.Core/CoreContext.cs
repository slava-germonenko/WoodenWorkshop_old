using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Configuration;

namespace WoodenWorkshop.Core;

public class CoreContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateUpdatedDate();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateUpdatedDate();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateUpdatedDate();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
    }

    private void UpdateUpdatedDate()
    {
        var updatedDate = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseModel>().Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.Updated = updatedDate;
        }
    }
}
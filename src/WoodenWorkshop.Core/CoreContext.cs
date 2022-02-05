using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Configuration;

namespace WoodenWorkshop.Core;

public class CoreContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PriceType> PriceTypes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductAssets> ProductAssets { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<ProductSocialLink> ProductSocialLinks { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        SetUpdatedDateAndDiscardCreatedDateChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        SetUpdatedDateAndDiscardCreatedDateChanges();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdatedDateAndDiscardCreatedDateChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AssetEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAssetConfiguration());
        modelBuilder.ApplyConfiguration(new ProductPriceConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
    }

    private void SetUpdatedDateAndDiscardCreatedDateChanges()
    {
        var commitDate = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseModel>().Where(e => e.State == EntityState.Modified))
        {
            entry.Property(entity => entity.Created).IsModified = false;
            entry.Entity.Updated = commitDate;
        }
        foreach (var entry in ChangeTracker.Entries<BaseModel>().Where(e => e.State == EntityState.Added))
        {
            entry.Entity.Created = commitDate;
        }
    }
}
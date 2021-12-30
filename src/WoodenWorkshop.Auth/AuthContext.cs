using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Auth;

public class AuthContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    
    public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Session>().HasKey(s => s.RefreshToken);
    }
}
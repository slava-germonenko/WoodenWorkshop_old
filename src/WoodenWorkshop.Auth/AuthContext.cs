using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Auth;

public class AuthContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    
    public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }
}
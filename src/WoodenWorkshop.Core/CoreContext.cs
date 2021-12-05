using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core;

public class CoreContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }
}
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Api.Data;

public class UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : DbContext(options)
{
  public DbSet<Models.User> Users { get; set; }
  public DbSet<Models.Group> Groups { get; set; }
  public DbSet<Models.UserGroup> UserGroups { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Models.UserGroup>().
        HasKey(x => new { x.UserId, x.GroupId });

    modelBuilder.Entity<Models.UserGroup>()
        .HasOne(x => x.User)
        .WithMany(x => x.Groups)
        .HasForeignKey(x => x.UserId);

    modelBuilder.Entity<Models.UserGroup>()
        .HasOne(x => x.Group)
        .WithMany(x => x.Users)
        .HasForeignKey(x => x.GroupId);

    base.OnModelCreating(modelBuilder);
  }


}

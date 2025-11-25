using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Models;

namespace UserManagement.Api.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
}

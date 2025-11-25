using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Models;

namespace UserManagement.Api.Data;

public class GroupDbContext :DbContext
{
    public GroupDbContext(DbContextOptions<GroupDbContext> options)
        : base(options) { }
    
    public DbSet<Group> Groups { get; set; }
}
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Models;
using UserManagement.Api.Utility;

namespace UserManagement.Api.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(UserManagementDbContext db)
    {
        await db.Database.MigrateAsync();

        await SeedGroups(db);
        SeedUsers(db);
        await SeedAdmin(db);
    }

    private static async Task SeedGroups(UserManagementDbContext db)
    {
        if (await db.Groups.AnyAsync())
        {
            return;
        }

        var groups = new[]
        {
            new Group { Id = Guid.NewGuid(), Name = "Admin" },
            new Group { Id = Guid.NewGuid(), Name = "HR" },
            new Group { Id = Guid.NewGuid(), Name = "IT" },
            new Group { Id = Guid.NewGuid(), Name = "Sales" }
        };

        db.Groups.AddRange(groups);

        await db.SaveChangesAsync();
    }

    private static void SeedUsers(UserManagementDbContext db)
    {
       //implement bogus users if needed in future
    }

    private static async Task SeedAdmin(UserManagementDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Email == "admin@local"))
        {
            return;
        }

        var adminUser = new User()
        {
            Id = Guid.NewGuid(),
            Name = "admin",
            Email = "admin@local",
            PasswordHash = PasswordHasher.HashPassword("admin"),
        };

        var adminGroup = await db.Groups.FirstAsync(x => x.Name == "Admin");
        
        if (adminGroup == null)
        {
            throw new Exception("How is there no admin group?");
        }

        db.Users.Add(adminUser);

        db.UserGroups.Add(new UserGroup()
        {
            GroupId = adminGroup.Id,
            UserId = adminUser.Id
        });

        await db.SaveChangesAsync();
    }
}
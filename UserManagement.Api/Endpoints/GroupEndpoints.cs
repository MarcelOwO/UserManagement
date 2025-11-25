using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;
using UserManagement.Core.Dto.Group;

namespace UserManagement.Api.Endpoints;

public static class GroupEndpoints
{

    public static void Map(WebApplication app)
    {
        var groups = app.MapGroup("/groups");

        groups.MapGet("/", async (UserManagementDbContext context) => await context.Groups.ToListAsync());

        groups.MapDelete("/delete/{id}", async (string id, UserManagementDbContext context) =>
        {
            var group = await context.Groups.FirstOrDefaultAsync(g => g.Id.ToString() == id);
            
            if (group == null)
            {
                return Results.NotFound();
            }

            context.Groups.Remove(group);
            
            await context.SaveChangesAsync();
            
            return Results.Ok();
        });

        groups.MapPost("/create", async (CreateGroupDto dto, UserManagementDbContext context) =>
        {
            var group = new Group()
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };
            
            await context.Groups.AddAsync(group);
            
            await context.SaveChangesAsync();
            
            return Results.Ok(group);
        });
        
        groups.MapPost("/update/{id}", async (string id, UpdateGroupDto dto, UserManagementDbContext context) =>
        {
            var group = await context.Groups.FirstOrDefaultAsync(g => g.Id.ToString() == id);
            
            if (group == null)
            {
                return Results.NotFound();
            }

            group.Name = dto.Name;
            
            context.Groups.Update(group);
            
            await context.SaveChangesAsync();
            
            return Results.Ok(group);
        });
    }
    
}
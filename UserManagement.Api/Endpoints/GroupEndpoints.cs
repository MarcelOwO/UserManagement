using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;

namespace UserManagement.Api.Endpoints;

public static class GroupEndpoints
{

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/groups");

        group.MapGet("/", async (GroupDbContext context) => await context.Groups.ToListAsync());
    }
    
}
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;

namespace UserManagement.Api.Endpoints;

public static class UserEndpoints
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/users");

        app.MapGet("/", async (UserDbContext context) => { await context.Users.ToListAsync(); });

        app.MapPost("/create", async (UserDbContext context, User user) => { await context.Users.AddAsync(user); });

        app.MapPost("/update/{id}", (UserDbContext context, User user) => { context.Users.Update(user); });
        
       app.MapDelete("/{id}", (string id, UserDbContext context) =>
       {
       }); 
    }
}
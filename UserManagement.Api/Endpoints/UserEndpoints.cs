using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;
using UserManagement.Core.Dto;
using UserManagement.Api.Utility;

namespace UserManagement.Api.Endpoints;

public static class UserEndpoints
{
  public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/users");

    group.MapGet("/", async (string? search, int page, int pageSize, UserManagementDbContext
        context) =>
    {
      page = page <= 0 ? 1 : page;
      pageSize = pageSize <= 0 ? 10 : pageSize;
      var query = context.Users.AsQueryable();

      if (!string.IsNullOrWhiteSpace(search))
      {
        query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
      }

      var users = await query.OrderBy(u => u.Name)
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();

      return Results.Ok(users);
    });

    group.MapPost("/create",
        async (UserManagementDbContext context, CreateUserDto dto) =>
        {
          var defaultPassword = RandomPasswordGenerator.Generate();

          var hash = PasswordHasher.HashPassword(defaultPassword);

          var user = new User()
          {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Name = dto.Name,
            PasswordHash = hash,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
          };

          await context.Users.AddAsync(user);

          await context.SaveChangesAsync();

          return Results.Ok();
        });

    group.MapPost("/update/{id}", async (string id, UpdateUserDto dto, UserManagementDbContext context) =>
    {
      var user = context.Users.FirstOrDefault(u => u.Id.ToString() == id);

      if (user == null)
      {
        return Results.NotFound();
      }

      user.Name = dto.Name;
      user.Email = dto.Email;

      context.Users.Update(user);

      await context.SaveChangesAsync();

      return Results.Ok();
    });

    group.MapDelete("/{id}", async (string id, UserManagementDbContext context) =>
    {
      var user = context.Users.FirstOrDefault(u => u.Id.ToString() == id);

      if (user == null)
      {
        return Results.NotFound();
      }

      context.Users.Remove(user);
      await context.SaveChangesAsync();

      return Results.Ok();
    });
    return app;
  }
}

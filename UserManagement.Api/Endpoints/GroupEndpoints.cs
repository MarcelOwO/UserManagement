using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;
using UserManagement.Core.Dto.Group;

namespace UserManagement.Api.Endpoints;

public static class GroupEndpoints
{
  public static IEndpointRouteBuilder MapGroupEndpoints(this IEndpointRouteBuilder app)
  {
    var groups = app.MapGroup("/groups").RequireAuthorization("admin");

    groups.MapGet("/", async (string? search, int? page, int? pageSize, UserManagementDbContext db) =>
    {
      IQueryable<Group> queryable = db.Groups.AsQueryable();

      if (!string.IsNullOrWhiteSpace(search))
      {
        var term = search.ToLower();
        queryable = queryable.Where(g => g.Name.ToLower().Contains(term));
      }

      int defaultPageSize = Math.Clamp(pageSize ?? 20, 1, 100);
      int defaultPage = Math.Max(1, page ?? 1);

      queryable = queryable.Skip((defaultPage - 1) * defaultPageSize).Take(defaultPageSize);

      return Results.Ok(await queryable.ToListAsync());
    });


    groups.MapGet("/{id:guid}", async (Guid id, UserManagementDbContext db) =>
    {
      var group = await db.Groups.AsNoTracking().FirstOrDefaultAsync();

      if (group == null)
      {
        return Results.NotFound();
      }

      return Results.Ok(group);
    });

    groups.MapPost("/create", async (CreateGroupDto dto, UserManagementDbContext db) =>
    {

      //  add more user checks like birthday or similar
      var exists = await db.Groups.AnyAsync(g => g.Name == dto.Name);
      if (exists)
      {
        return Results.Conflict($"Group with those details already exists");
      }

      var group = new Group()
      {
        Id = Guid.NewGuid(),
        Name = dto.Name
      };

      await db.Groups.AddAsync(group);
      await db.SaveChangesAsync();

      return Results.Created($"/groups/{group.Id}", group);
    });

    groups.MapPost("/update/{id:guid}", async (Guid id, UpdateGroupDto dto, UserManagementDbContext db) =>
    {

      bool nameTaken = await db.Groups.AnyAsync(g => g.Name == dto.Name && g.Id != id);

      if (nameTaken)
      {
        return Results.Conflict($"Group with that name already exists");
      }

      var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == id);

      if (group == null)
      {
        return Results.NotFound();
      }

      group.Name = dto.Name;

      db.Groups.Update(group);

      await db.SaveChangesAsync();

      return Results.Ok(group);
    });

    groups.MapDelete("/delete/{id:guid}", async (Guid id, UserManagementDbContext db) =>
    {
      var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == id);

      if (group == null)
      {
        return Results.NotFound();
      }

      db.Groups.Remove(group);

      await db.SaveChangesAsync();

      return Results.Ok();
    }).RequireAuthorization("admin");
    return app;
  }

}

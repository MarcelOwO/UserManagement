using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;

namespace UserManagement.Api.Endpoints;

public static class ManagementEndpoints
{
  public static IEndpointRouteBuilder MapManagementEndpoints(this IEndpointRouteBuilder app)
  {
    var groups = app.MapGroup("/api/management").RequireAuthorization("admin");

    groups.MapPost("/groups/{groupId:guid}/users/{userId:guid}", async (Guid groupId, Guid userId, UserManagementDbContext db) =>
    {
      var userExists = await db.Users.AnyAsync(u => u.Id == userId);
      var groupExists = await db.Groups.AnyAsync(g => g.Id == groupId);

      if (!userExists || !groupExists)
      {
        return Results.NotFound("User or group not found");
      }

      var existingAssignment = await db.UserGroups.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);

      if (existingAssignment != null)
      {
        return Results.Conflict("This user is already a member of this group");
      }

      var assignment = new UserGroup
      {
        UserId = userId,
        GroupId = groupId,
      };

      await db.UserGroups.AddAsync(assignment);

      await db.SaveChangesAsync();

      return Results.Created($"/api/management/groups/{groupId:guid}/users/{userId:guid}", null);
    });

    groups.MapDelete("/groups/{groupId:guid}/users/{userId:guid}", async (Guid groupId, Guid userId, UserManagementDbContext db) =>
    {
      var affected = await db.UserGroups
      .Where(ug => ug.GroupId == groupId && ug.UserId == userId)
      .ExecuteDeleteAsync();

      if (affected == 0)
      {
        return Results.NotFound("The user is not currently assigned to that group");
      }

      return Results.NoContent();
    });

    return app;
  }



}

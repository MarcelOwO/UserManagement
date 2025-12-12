using UserManagement.Api.Data;

namespace UserManagement.Api.Endpoints;

public static class ManagementEndpoints
{

  public static IEndpointRouteBuilder MapManagementEndpoints(this IEndpointRouteBuilder app)
  {
    var groups = app.MapGroup("/api");

    groups.MapPost("/{groupId:guid}/users/{userId:guid}", async (Guid groupId, Guid userId, UserManagementDbContext db) =>
    {

    });

    return app;


  }



}

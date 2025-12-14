using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;
using UserManagement.Core;
using UserManagement.Core.Dto.Group;
using UserManagement.Core.Dto.User;

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

            var totalCount = await queryable.CountAsync();

            var groups = await queryable
                .OrderBy(g => g.Name)
                .Skip((defaultPage - 1) * defaultPageSize)
                .Take(defaultPageSize)
                .Select(g => new GroupDto(g.Id, g.Name)).ToListAsync();

            return Results.Ok(new PaginatedResult<GroupDto>()
            {
                Items = groups,
                Page = defaultPage,
                PageSize = defaultPageSize,
                TotalCount = totalCount
            });
        });

        groups.MapGet("/{id:guid}", async (Guid id, UserManagementDbContext db) =>
        {
            var group = await db.Groups
                .Where(g => g.Id == id)
                .Include(u => u.Users)
                .ThenInclude(ug => ug.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return Results.NotFound();
            }

            var groupDto = new GroupWithUsersDto(
                group.Id,
                group.Name,
                group.Users.Select(u => new UserDto(u.User.Id, u.User.Email, u.User.Name)).ToList()
            );

            return Results.Ok(groupDto);
        });

        groups.MapPost("/", async (CreateGroupDto dto, UserManagementDbContext db) =>
        {
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

        groups.MapPost("/{id:guid}", async (Guid id, UpdateGroupDto dto, UserManagementDbContext db) =>
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

            await db.SaveChangesAsync();

            return Results.Ok(group);
        });

        groups.MapDelete("/delete/{id:guid}", async (Guid id, UserManagementDbContext db) =>
        {
            var affected = await db.Groups
                .Where(g => g.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        });
        return app;
    }
}
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Models;
using UserManagement.Core.Dto;
using UserManagement.Api.Utility;
using UserManagement.Core;
using UserManagement.Core.Dto.User;
using UserManagement.Core.Dto.Group;

namespace UserManagement.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").RequireAuthorization("admin");

        group.MapGet("/", async (string? search, int? page, int? pageSize, UserManagementDbContext
            db) =>
        {
            var queryable = db.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                queryable = queryable.Where(u => u.Name.ToLower().Contains(term));
            }

            int defaultPageSize = Math.Clamp(pageSize ?? 20, 1, 100);
            int defaultPage = Math.Max(1, page ?? 1);

            var totalCount = await queryable.CountAsync();

            var users = await queryable
                .OrderBy(u => u.Name)
                .Skip((defaultPage - 1) * defaultPageSize)
                .Take(defaultPageSize)
                .Select(u => new UserDto(u.Id, u.Email, u.Name))
                .ToListAsync();

            return Results.Ok(new PaginatedResult<UserDto>()
            {
                Items = users,
                Page = defaultPage,
                PageSize = defaultPageSize,
                TotalCount =totalCount 
            });
        });

        group.MapGet("/{id:guid}", async (Guid id, UserManagementDbContext db) =>
        {
            var user = await db.Users
                .Where(u => u.Id == id)
                .Include(g => g.Groups)
                .ThenInclude(ug => ug.Group)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Results.NotFound();
            }

            var userDto = new UserWithGroupsDto(user.Id,
                user.Email,
                user.Name,
                user.Groups.Select(g => new GroupDto(g.GroupId, g.Group.Name)).ToList());

            return Results.Ok(userDto);
        });

        //  create user
        group.MapPost("/",
            async (UserManagementDbContext db, CreateUserDto dto) =>
            {
                if (await db.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    return Results.Conflict("A user with this email already exists");
                }

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

                await db.Users.AddAsync(user);

                await db.SaveChangesAsync();

                return Results.Created($"/users/{user.Id}", user);
            });

        group.MapPost("/{id:guid}", async (Guid id, UpdateUserDto dto, UserManagementDbContext db) =>
        {
            var user = db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return Results.NotFound();
            }

            if (user.Email != dto.Email && await db.Users.AnyAsync(u => u.Id == id))
            {
                return Results.Conflict("The new email is already used");
            }

            user.Name = dto.Name;
            user.Email = dto.Email;

            await db.SaveChangesAsync();

            return Results.Ok();
        });

        group.MapDelete("/{id:guid}", async (Guid id, UserManagementDbContext db) =>
        {
            var affected = await db.Users
                .Where(u => u.Id == id)
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
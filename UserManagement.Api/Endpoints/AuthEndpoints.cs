using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Api.Data;
using UserManagement.Api.Utility;
using UserManagement.Core.Dto;
using UserManagement.Core.Dto.Auth;

namespace UserManagement.Api.Endpoints;

public static class AuthEndpoints
{
    public static void Map(WebApplication app,string jwtSecret)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", (LoginDto dto, UserManagementDbContext context) =>
        {
            var user = context.Users.FirstOrDefault(u => u.Email == dto.Email);
            
            if (user == null)
            {
                return Results.NotFound();
            }

            if (!PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", user.Id.ToString()),
                    new System.Security.Claims.Claim("email", user.Email),
                    new System.Security.Claims.Claim("name", user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Results.Ok(new{Token = tokenHandler.WriteToken(token)});
        });

    }
    
}
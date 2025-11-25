using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Api.Data;
using UserManagement.Core.Dto;

namespace UserManagement.Api.Endpoints;

public static class AuthEndpoints
{
    public static void Map(WebApplication app,string jwtSecret)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", (LoginDto login,UserDbContext context) =>
        {

            var user = context.Users.FirstOrDefault(u => u.Email == login.Email && u.PasswordHash == login.Password);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            
            var tokenDescriptor = new SecurityTokenDescriptor()
            {


            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            

            return Results.Ok(new{Token = tokenHandler.WriteToken(token)});
        });

    }
    
}
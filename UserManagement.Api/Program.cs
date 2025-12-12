using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Data.Seed;
using UserManagement.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration.GetValue<string>("JwtKey") ?? throw new Exception("JWT Key is missing");

builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserManagementDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("UserManagementDb");
  options.UseSqlServer(connectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtKey)
            )
      };
    });

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin", policy =>
  {
    policy.RequireClaim("admin", "true");
  });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
  app.UseSwaggerUI();
  app.UseSwagger();
  app.MapOpenApi();
}

app.MapAuthEndpoints(jwtKey)
.MapGroupEndpoints()
.MapUserEndpoints()
.MapManagementEndpoints();

await app.MigrateDatabaseAsync();

app.Run();

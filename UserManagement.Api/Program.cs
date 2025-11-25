using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Data.Seed;
using UserManagement.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<UserManagementDbContext>(options=>
{
    var connectionString = builder.Configuration.GetConnectionString("UserManagementDb");
    options.UseSqlServer(connectionString);

});

var jwtKey = builder.Configuration.GetValue<string>("JwtKey");

if (jwtKey == null)
{
    throw new Exception("JWT Key is missing");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    });

builder.Services.AddAuthorizationBuilder();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapSwagger().RequireAuthorization();

UserEndpoints.Map(app);
GroupEndpoints.Map(app);
AuthEndpoints.Map(app,jwtKey );

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
await DatabaseSeeder.SeedAsync(dbContext);

app.Run();

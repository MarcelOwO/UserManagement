using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddDbContext<UserDbContext>(options=>
{
    var connectionString = builder.Configuration.GetConnectionString("UserDb");
    options.UseSqlServer(connectionString);

});
builder.Services.AddDbContext<GroupDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("GroupDb");
    options.UseSqlServer(connectionString);

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

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

app.Run();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);

var userManagementDb = sql.AddDatabase("UserManagementDb");

var api = builder
    .AddProject<UserManagement_Api>("api")
    .WithEnvironment("JWT_SECRET","supersecretkey")
    .WithReference(userManagementDb)
    .WaitFor(userManagementDb);

var web = builder
    .AddProject<UserManagement_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();

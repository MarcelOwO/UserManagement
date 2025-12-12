using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);

var userManagementDb = sql.AddDatabase("UserManagementDb");

var api = builder
    .AddProject<UserManagement_Api>("Api")
    .WithReference(userManagementDb)
    .WaitFor(userManagementDb);

var web = builder
    .AddProject<UserManagement_Web>("Web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();

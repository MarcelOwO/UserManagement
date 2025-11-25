using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var userDb = sql.AddDatabase("UserDb");

var groupDb = sql.AddDatabase("GroupDb");

var api = builder.AddProject<UserManagement_Api>("Api")
    .WithReference(userDb)
    .WithReference(groupDb)
    .WaitFor(userDb)
    .WaitFor(groupDb);

var web =  builder.AddProject<UserManagement_Web>("Web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

// Register app projects

var cache = builder.AddRedis("cache");  // Run Redis (.NET Aspire launches Redis in Docker)

var api = builder.AddProject<Projects.WebAPI>("webapi");

builder.AddProject<Projects.BlazorApp>("ui")
    .WithExternalHttpEndpoints()    // Enable external http endpoints
    .WithReference(api)
    .WaitFor(api)
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();

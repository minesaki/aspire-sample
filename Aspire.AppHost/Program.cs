var builder = DistributedApplication.CreateBuilder(args);

// Register app projects

var db = builder.AddPostgres("postgres")            // Run PostgreSQL in Docker
    .WithEnvironment("POSTGRES_DB", "weatherdb")    // Create DB "weatherdb" by passing env var
    .WithInitBindMount("./db-data")                 // Mount directory containing init script(s). Automatically executed when PostgreSQL starts
    .WithPgAdmin()                                  // Use pgAdmin
    .PublishAsConnectionString()                    // When published, instead of deploying PostgreSQL container, use connection string (env var ConnectionStrings__weatherdb)
    .AddDatabase("weatherdb");                      // Add reference to the database "weatherdb"

var api = builder.AddProject<Projects.WebAPI>("webapi")
    .WithReference(db).WaitFor(db);                 // Add reference to DB and wait for it

var cache = builder.ExecutionContext.IsPublishMode
    ? builder.AddConnectionString("cache")  // If published, use existing Redis (don't deploy Redis)
    : builder.AddRedis("cache");  // If in local, run Redis (.NET Aspire launches Redis in Docker)

builder.AddProject<Projects.BlazorApp>("ui")
    .WithExternalHttpEndpoints()    // Enable external http endpoints
    .WithReference(api).WaitFor(api)                // Add reference to API and wait for it
    .WithReference(cache).WaitFor(cache);           // Add reference to cache and wait for it

// Jaeger tracing
// builder.AddContainer("jaeger", "jaegertracing/all-in-one")
//     .WithHttpEndpoint(16686, targetPort: 16686, name: "jaegerPortal")
//     .WithHttpEndpoint(4317, targetPort: 4317, name: "jaegerEndpoint");

builder.Build().Run();

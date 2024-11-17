using BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();   // .NET Aspire

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient
builder.Services.AddHttpClient<WebAPIClient>(client =>
{
    client.BaseAddress = new Uri("http://webapi");  // using service discovery provided by .NET Aspire
});

builder.AddRedisOutputCache("cache");  // Output cache with Redis provided by .NET Aspire

var app = builder.Build();

app.MapDefaultEndpoints();   // .NET Aspire

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();       // Disable https


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseOutputCache();       // Redis output cache

app.Run();

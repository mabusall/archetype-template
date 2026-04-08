WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Used only for Web API Projects, so we can add it before UseInfrastructureServices
builder.AddApiServiceDefaults();

// Add Tasheer Core Module and Infrastructure Services
builder.AddSampleCoreModule();
builder.AddInfrastructureServices();

WebApplication app = builder.Build();

// Add security headers middleware (BEFORE other middleware)
// Note: These are API-specific headers. Many browser-focused headers are omitted
// because this is a backend API accessed by frontend apps, not browsers directly.
app.Use(async (context, next) =>
{
    // X-Content-Type-Options: Prevents MIME type sniffing (defense in depth)
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Strict-Transport-Security (HSTS): Forces HTTPS connections
    // Important for APIs to prevent man-in-the-middle attacks
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security",
            "max-age=31536000; includeSubDomains");
    }

    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Tasheer API")
            .WithTheme(ScalarTheme.Kepler)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    // Add OpenAPI spec export endpoint for frontend teams
    // The MapOpenApi() above already creates the endpoint at /openapi/v{documentName}.json
    // Frontend teams can access it at: https://localhost:5001/openapi/v1.json
    // Download with curl: curl https://localhost:5001/openapi/v1.json -o openapi.json -k
    // 
    // For production access, move MapOpenApi() outside the Development check
    // and add authentication if needed
}

app
    .UseInfrastructureServices(true)
    .UseTasheerCoreModule();

// cleanup memory stack
MemeoryStack.Flush();

await app.RunAsync();
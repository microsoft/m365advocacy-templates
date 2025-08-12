using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using CustomEngineAgent;
using CustomEngineAgent.Services;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;
using Microsoft.Agents.Storage.Blobs;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();
builder.Services.AddCloudAdapter();
builder.Logging.AddConsole();

if (builder.Environment.IsProduction()) {
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder => 
            tracerProviderBuilder.AddHttpClientInstrumentation(options => {
                // Configure HTTP client instrumentation for better OpenAI telemetry
                options.RecordException = true;
                options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) => {
                    activity.SetTag("http.request.header.user-agent", httpRequestMessage.Headers.UserAgent?.ToString());
                };
                options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) => {
                    activity.SetTag("http.response.status_code", (int)httpResponseMessage.StatusCode);
                };
            }))
        .UseAzureMonitor(options => {
            options.Credential = new DefaultAzureCredential();
        });
}

// Add AspNet token validation
builder.Services.AddBotAspNetAuthentication(builder.Configuration);

builder.Services.AddSingleton<IStorage>((sp) => {
    var containerName = builder.Configuration["BlobsStorageOptions:ContainerName"] ?? "state";

    if (builder.Environment.IsDevelopment())
    {
        // Use Azurite for local development storage
        return new BlobsStorage("UseDevelopmentStorage=true", containerName);
    }
    else
    {
        // Use managed identity for production
        var storageAccountName = builder.Configuration["BlobsStorageOptions:StorageAccountName"];
        return new BlobsStorage(
            new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}"), 
            new DefaultAzureCredential()
        );
    }
});

// Add Chat Client Service (supports both OpenAI and Azure OpenAI)
builder.Services.AddSingleton(sp => ChatClientFactory.CreateChatClientService(builder.Configuration));

// Add AgentApplicationOptions from config.
builder.AddAgentApplicationOptions();

// Add the bot (which is transient)
builder.AddAgent<Bot>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map the /api/messages endpoint to the AgentApplication
app.MapPost("/api/messages", async (HttpRequest request, HttpResponse response, IAgentHttpAdapter adapter, IAgent agent, CancellationToken cancellationToken) =>
{
    await adapter.ProcessAsync(request, response, agent, cancellationToken);
});

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Playground")
{
    app.MapGet("/", () => "Echo Agent");
    app.UseDeveloperExceptionPage();
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

app.Run();
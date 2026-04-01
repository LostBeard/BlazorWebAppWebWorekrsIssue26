using BlazorWebApp10.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebWorkers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// SpawnDev.BlazorJS setup
builder.Services.AddBlazorJSRuntime();

// WebWorkerService
builder.Services.AddWebWorkerService();

// Test service that will be called in the Window context and run in the SharedWebWorker context
// See Home.razor for example usage
builder.Services.AddSingleton<ITestService, TestService>();

await builder.Build().BlazorJSRunAsync();

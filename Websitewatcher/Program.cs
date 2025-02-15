using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Websitewatcher;
using Websitewatcher.Services;

//var builder = FunctionsApplication.CreateBuilder(args);

//builder.ConfigureFunctionsWebApplication();

//// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
//// builder.Services
////     .AddApplicationInsightsTelemetryWorkerService()
////     .ConfigureFunctionsApplicationInsights();

//builder.Build().Run();
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(app =>
    {
        app.UseWhen<SafeBrowsingMiddleware>(context =>
        {
            return context.FunctionDefinition.Name == "Register";
        });
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<PdfCreaterService>();
        services.AddSingleton<SafeBrowsingService>();
    })
    .Build();

host.Run();


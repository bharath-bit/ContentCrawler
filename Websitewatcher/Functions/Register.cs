using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Websitewatcher.Services;

namespace Websitewatcher.Functions;

public class Register(ILogger<Register> logger/*, SafeBrowsingService safeBrowsingService*/)
{
    private readonly ILogger<Register> _logger = logger;

    [Function(nameof(Register))]
    [SqlOutput("dbo.website", "websitewatcher")]
    public async Task<website> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var newWebsite = JsonSerializer.Deserialize<website>(requestBody, options);
        newWebsite.ID = Guid.NewGuid();
        //var result = safeBrowsingService.Check(newWebsite.Url);
        //if (result.HasThreat)
        //{
        //    var threats = string.Join("", result.Threats);
        //    logger.LogError($"Url has the following threats: {threats}");
        //    return null;
        //}
        return newWebsite;
    }
    public class website
    {
        public Guid ID { get; set; }
        public string Url { get; set; }
        public string? xpath { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Websitewatcher;

public class query(ILogger<query> logger)
{
    private const string querystring = @"
    SELECT 
        w.id,
        w.url,
        s.[Timestamp] AS latesttimestamp
    FROM dbo.website w
    LEFT JOIN dbo.snapshot s 
        ON w.id = s.id
    WHERE s.[Timestamp] = (
            SELECT MAX([Timestamp]) 
            FROM dbo.snapshot 
            WHERE id = w.id
        )
    AND s.[Timestamp] BETWEEN DATEADD(HOUR, -3, GETUTCDATE()) AND GETUTCDATE()
";


    [Function(nameof(query))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, [SqlInput(querystring, "websitewatcher")] IReadOnlyList<dynamic> websites  )
    {
        //_logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(websites);
    }
}

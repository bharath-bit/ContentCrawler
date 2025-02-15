using Google.Apis.Safebrowsing.v4;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Websitewatcher.Services;

namespace Websitewatcher;

public class SafeBrowsingMiddleware(SafeBrowsingService safebrowsingservice) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var request = await context.GetHttpRequestDataAsync();
       if( !context.BindingContext.BindingData.ContainsKey("Url"))
        {
           var response= request!.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await response.WriteStringAsync("You must specify the URL");
            return;
        }
       var url = context.BindingContext.BindingData["Url"]?.ToString();
        if (!isvalidurl(url))
        {
            var response = request!.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await response.WriteStringAsync("Bad URL format");
            return;
        }
        var safecheckresult= safebrowsingservice.Check(url);
        if (safecheckresult.HasThreat)
        {
            var response = request!.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await response.WriteStringAsync(string.Join(" ", safecheckresult.Threats));
            return;
        }
        else
        {
            await next(context);
        }

    }
    private bool isvalidurl (string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriresult) && (uriresult.Scheme == Uri.UriSchemeHttp || uriresult.Scheme == Uri.UriSchemeHttps);
    }
}

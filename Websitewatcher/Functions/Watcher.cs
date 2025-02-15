using System;
using Azure.Storage.Blobs;
using HtmlAgilityPack;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Websitewatcher.Services;
namespace Websitewatcher.Functions;
public class Watcher(ILogger<Watcher> logger, PdfCreaterService pdfCreaterService)
{
    private const string querystring = @"SELECT w.id, w.url, w.xpath, s.content AS latestcontent
FROM dbo.website w
LEFT JOIN dbo.snapshot s 
    ON w.id = s.id
    AND s.timestamp = (SELECT MAX(timestamp) FROM dbo.snapshot WHERE id = w.id)";

    [Function(nameof(Watcher))]
    [SqlOutput("dbo.snapshot", "websitewatcher")]
    public async Task<snapshotrecord?> Run([TimerTrigger("*/20 * * * * *")] TimerInfo myTimer, [SqlInput(querystring, "websitewatcher")] IReadOnlyList<websitemodel> websites)

    {
        snapshotrecord result = null;
        foreach (var website in websites)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(website.Url);//this loads the page and saves in doc 
            var divwithcontent = doc.DocumentNode.SelectSingleNode(website.xpath);
            var content = divwithcontent != null ? divwithcontent.InnerText.Trim() : "No Content";
            // this is for testing 
            content = content.Replace("Microsoft Entra", "Azure AD");
            var contenthaschanged = content != website.LatestContent;
            if (contenthaschanged)
            {
                logger.LogInformation("Content changed !");

                var newpdf = await pdfCreaterService.ConvertpagetoPdfasync(website.Url);
                var connectionstring = Environment.GetEnvironmentVariable("ConnectionStrings:websitewatcherstorage");
                var blobclient = new BlobClient(connectionstring, "pdfs", $"{website.ID} -{DateTime.UtcNow:MMddyyyyhhmmss}.pdf");
                var blob = await blobclient.UploadAsync(newpdf);
                logger.LogInformation("new Pdf Uploaded");
                result = new snapshotrecord(website.ID, content);
            }

        }
        return result;

    }

}
public class websitemodel
{
    public Guid ID { get; set; }
    public string Url { get; set; }
    public string? xpath { get; set; }
    public string LatestContent { get; set; }
}

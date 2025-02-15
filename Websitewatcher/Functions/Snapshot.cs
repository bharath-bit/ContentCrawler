using HtmlAgilityPack;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using static Websitewatcher.Functions.Register;

namespace Websitewatcher.Functions;

public class Snapshot(ILogger<Snapshot> logger)
{

    [Function(nameof(Snapshot))]
    [SqlOutput("dbo.snapshot", "websitewatcher")]
    public snapshotrecord? Run(
        [SqlTrigger("dbo.website", "websitewatcher")] IReadOnlyList<SqlChange<website>> changes)
    {
        snapshotrecord result = null;
        foreach (var change in changes)
        {
            logger.LogInformation($"{change.Operation}");
            logger.LogInformation($"ID: {change.Item.ID} URL: {change.Item.Url}");
            if (change.Operation != SqlChangeOperation.Insert)
            {
                continue;
            }
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(change.Item.Url);//this loads the page and saves in doc 
            var divwithcontent = doc.DocumentNode.SelectSingleNode(change.Item.xpath);
            var content = divwithcontent != null ? divwithcontent.InnerText.Trim() : "No Content";
            logger.LogInformation(content);
            result = new snapshotrecord(change.Item.ID, content);

            //HttpClient client = new HttpClient();
        }
        return result;

    }

}
public record snapshotrecord(Guid ID, string Content);


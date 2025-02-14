using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using static Websitewatcher.Register;

namespace Websitewatcher;

public class PDFCreator(ILogger<PDFCreator> logger)
{
   
    [Function(nameof(PDFCreator))]
    [BlobOutput("pdfs/new.pdf",Connection ="websitewatcherstorage")]
    public async Task<byte[]?> Run(
        [SqlTrigger("[dbo].[website]", "websitewatcher")] SqlChange<website>[] changes)
    {
        byte[]? buffer= null;
        foreach (var change in changes)
        {
            if (change.Operation == SqlChangeOperation.Insert)
            {
                var result=ConvertpagetoPdfasync(change.Item.Url);
                buffer=new byte[result.Result.Length];
                await result.Result.ReadAsync(buffer);

                logger.LogInformation($"PDF Stream Length is: {result.Result.Length}");
            }
        }
        return buffer;

    }
    private async Task<Stream> ConvertpagetoPdfasync(string url)
    {
        var browserfetcher = new BrowserFetcher();
        await browserfetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        await page.EvaluateExpressionAsync("document.fonts.ready");
        var result=await page.PdfStreamAsync();
        result.Position = 0;
        return result;
    }
}



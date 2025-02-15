using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Websitewatcher.Services;
using static Websitewatcher.Functions.Register;

namespace Websitewatcher.Functions;

public class PDFCreator(ILogger<PDFCreator> logger, PdfCreaterService pdfCreaterService)
{

    [Function(nameof(PDFCreator))]
    [BlobOutput("pdfs/new.pdf", Connection = "websitewatcherstorage")]
    public async Task<byte[]?> Run(
        [SqlTrigger("[dbo].[website]", "websitewatcher")] SqlChange<website>[] changes)
    {
        byte[]? buffer = null;
        foreach (var change in changes)
        {
            if (change.Operation == SqlChangeOperation.Insert)
            {
                var result = pdfCreaterService.ConvertpagetoPdfasync(change.Item.Url);
                buffer = new byte[result.Result.Length];
                await result.Result.ReadAsync(buffer);

                logger.LogInformation($"PDF Stream Length is: {result.Result.Length}");
            }
        }
        return buffer;

    }

}



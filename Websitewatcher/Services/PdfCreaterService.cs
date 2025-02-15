using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Websitewatcher.Services;

public class PdfCreaterService
{
    public async Task<Stream> ConvertpagetoPdfasync(string url)
    {
        var browserfetcher = new BrowserFetcher();
        await browserfetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        await page.EvaluateExpressionAsync("document.fonts.ready");
        var result = await page.PdfStreamAsync();
        result.Position = 0;
        return result;
    }
}

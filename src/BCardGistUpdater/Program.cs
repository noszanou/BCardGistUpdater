using BCardGistUpdater.BCard;
using BCardGistUpdater.Extension;
using BCardGistUpdater.GistUpdater;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Za.NosGame.Fetcher;
using Za.NosGame.Fetcher.Downloader;
using Za.NosGame.Fetcher.Extractor;
using Za.NosGame.RessourceLoader.I18N;
using Za.NosGame.Shared;
using Za.NosGame.Shared.Loggers;

namespace BCardGistUpdater
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var services = new ServiceCollection().AddServices().BuildServiceProvider();
            await services.GetRequiredService<FileFetcher>().ExecuteAsync();
            await services.GetRequiredService<IBCardExtractorToJson>().Load();
            await services.GetRequiredService<IUpdater>().UpdateGistAsync();
        }
    }
}

using BCardGistUpdater.BCard;
using BCardGistUpdater.GistUpdater;
using Microsoft.Extensions.DependencyInjection;
using Za.NosGame.Fetcher;
using Za.NosGame.Fetcher.Downloader;
using Za.NosGame.Fetcher.Extractor;
using Za.NosGame.RessourceLoader.I18N;
using Za.NosGame.Shared;
using Za.NosGame.Shared.Loggers;

namespace BCardGistUpdater.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IBaseLogger, SerilogLogger>();
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<IBaseLogger>();
                var folder = new DatFileFolder(logger)
                {
                    RessourceFolder = "Ressource/"
                };
                return folder;
            });
            services.AddSingleton(new NosExtractorConfig(true, false, false, false, true, false));
            services.AddTransient<INosExtractor, NosExtractor>();
            services.AddTransient<IClientDownloader, ClientDownloader>();
            services.AddHttpClient();
            services.AddSingleton<FileFetcher>();

            services.AddSingleton<I18NImporter>();
            services.AddSingleton<II18NManager, I18NManager>();
            services.AddSingleton<IBCardExtractorToJson, BCardExtractorToJson>();

            services.AddSingleton<IUpdater, Updater>();
            return services;
        }
    }
}
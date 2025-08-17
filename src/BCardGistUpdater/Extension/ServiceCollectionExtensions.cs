using BCardGistUpdater.BCard;
using BCardGistUpdater.GistUpdater;
using BCardGistUpdater.GithubUpdater;
using Microsoft.Extensions.DependencyInjection;
using Za.NosGame.Fetcher;
using Za.NosGame.Fetcher.Downloader;
using Za.NosGame.Fetcher.Extractor;
using Za.NosGame.RessourceLoader._Extension;
using Za.NosGame.RessourceLoader.Manager;
using Za.NosGame.Shared;
using Za.NosGame.Shared.Loggers;

namespace BCardGistUpdater.Extension
{
    public static class ServiceCollectionExtensions
    {
        private static readonly bool useDefaultManager = true;
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IBaseLogger, SerilogLogger>();
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<IBaseLogger>();
                var folder = new DatFileFolder(logger)
                {
                    RessourceFolder = Path.Combine(AppContext.BaseDirectory, "Ressources")
                };
                return folder;
            });
            services.AddSingleton(new ConfigManager(useDefaultManager));
            services.AddSingleton(new NosExtractorConfig(true, true, false, false, true, false));
            services.AddTransient<INosExtractor, NosExtractor>();
            services.AddTransient<IClientDownloader, ClientDownloader>();
            services.AddHttpClient();
            services.AddSingleton<FileFetcher>();
            services.AddTraductionRessourceServices(useDefaultManager);
            services.AddSkillRessourceServices(useDefaultManager);
            services.AddCardRessourceServices(useDefaultManager);
            services.AddItemRessourceServices(useDefaultManager);
            services.AddNpcMonsterRessourceServices(useDefaultManager);
            services.AddBCardRessourceServices(useDefaultManager);
            services.AddSingleton<IBCardExtractorToJson, BCardExtractorToJson>();

            services.AddSingleton<IUpdater, Updater>();
            services.AddSingleton<IGitHubPRUpdater, GitHubPRUpdater>();
            return services;
        }
    }
}
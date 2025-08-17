using BCardGistUpdater.BCard;
using BCardGistUpdater.Extension;
using BCardGistUpdater.GistUpdater;
using BCardGistUpdater.GithubUpdater;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Za.NosGame.Fetcher;
using Za.NosGame.RessourceLoader.Manager;
using Za.NosGame.Shared;
using Za.NosGame.Shared.DatEntitys.Entitys;

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

            var skillManager = services.GetRequiredService<IGameEntityManager<Skill>>();
            var cardManager = services.GetRequiredService<IGameEntityManager<Card>>();
            var itemManager = services.GetRequiredService<IGameEntityManager<Item>>();
            var npcMonsterManager = services.GetRequiredService<IGameEntityManager<NpcMonster>>();
            var bcardManager = services.GetRequiredService<IGameEntityManager<BCardLang>>();
            await skillManager.InitializeAsync();
            await cardManager.InitializeAsync();
            await itemManager.InitializeAsync();
            await npcMonsterManager.InitializeAsync();
            await bcardManager.InitializeAsync();
            await services.GetRequiredService<IGitHubPRUpdater>().UpdateJsonFile();
        }
    }
}

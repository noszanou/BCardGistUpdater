using BCardGistUpdater.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Za.NosGame.Shared;

namespace BCardGistUpdater.GistUpdater
{
    public class Updater : IUpdater
    {
        private readonly DatFileFolder _datFileFolder;
        private const string GistId = "04f87cbde1db24870b72c88048f70bf3";
        private const string FileName = "BCard_EN.json";
        public Updater(DatFileFolder datFileFolder)
        {
            _datFileFolder = datFileFolder;
        }

        public async Task UpdateGistAsync()
        {
            string bcardFolder = Path.Combine(_datFileFolder.RessourceFolder, "BcardJSON", FileName);

            if (!File.Exists(bcardFolder))
            {
                return;
            }
            string newContent = await File.ReadAllTextAsync(bcardFolder);

            string token = Environment.GetEnvironmentVariable("TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNet-Gist-Updater");

            bool changed = await GitHubUpdateHelper.UpdateGistIfChangedAsync(client, GistId, FileName, newContent);

            if (!changed)
            {
                return;
            }

            var files = new Dictionary<string, object>
            {
                { FileName, new { content = newContent } }
            };
            var payload = new Dictionary<string, object>
            {
                { "files", files }
            };

            var json = JsonConvert.SerializeObject(payload);
            var response = await client.PatchAsync(
                $"https://api.github.com/gists/{GistId}",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
        }
    }
}